using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Combat;
using UnityEngine.Events;

namespace PSmash.Attributes
{
    public class EnemyPosture : MonoBehaviour
    {
        public float posture = 50;
        [SerializeField] float timeToCanRegen = 1f;
        [SerializeField] float timeToFullyRegenGuard = 60f;
        [SerializeField] UnityEvent onStunStateStart;
        [SerializeField] UnityEvent onStunStateEnded;


        float timerCanRegen = 0;
        float initialPosture;

        private void Awake()
        {
            initialPosture = posture;
        }

        private void Update()
        {
            if (posture == 0) 
                return;
            if( posture < initialPosture && timerCanRegen > timeToCanRegen)
            {
                posture = RegenPosture();
            }
            UpdateTimers();
        }

        private float RegenPosture()
        {
            float percentage = posture / initialPosture;
            percentage += Time.deltaTime / timeToFullyRegenGuard;
            if (percentage > 1) percentage = 1;
            percentage = percentage * initialPosture;
            return percentage;
        }

        void UpdateTimers()
        {
            timerCanRegen += Time.deltaTime;
        }

        public float SubstractDamageFromPosture(float damage)
        {
            float posture = this.posture;
            posture -= damage;
            if (posture <= 0) posture = 0;
            return posture;
        }

        public float GetPosture()
        {
            return posture;
        }

        public float GetInitialPosture()
        {
            return initialPosture;
        }
        
        public void OnStunStateStart()
        {
            onStunStateStart.Invoke();
        }

        //UnityEvent (EnemyHealth)
        public void FullyRegenPosture()
        {
            //print("Posture Fully Restored");
            posture = initialPosture;
            onStunStateEnded.Invoke();
        }
    }

}

