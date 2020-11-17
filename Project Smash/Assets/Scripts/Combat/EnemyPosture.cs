using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Resources
{
    public class EnemyPosture : MonoBehaviour
    {
        public enum CurrentActionsWhenDamaged
        {
            NormalAttacking,
            UnblockableAttacking,
            Parrying,
            NoAttackAction,
        }

        [SerializeField] float posture = 50;
        [SerializeField] float timerThreshold = 1f;
        [SerializeField] float timeToFullyRegenGuard = 60f;

        public event Action OnGuardBarRecoveredAfterStun;

        EnemyHealth health;
        Coroutine coroutine;

        float initialPosture;
        bool canRegen = true;
        float timer = 0;
        CurrentActionsWhenDamaged myActions;

        private void Awake()
        {
            health = GetComponent<EnemyHealth>();
            initialPosture = posture;
        }
        // Start is called before the first frame update

        private void Update()
        {
            if (health.IsDead() || health.IsStunned())
            {
                posture = 0;
                return;
            }
            if (canRegen && posture < initialPosture)
            {
                float percentage = (float)posture / (float)initialPosture;
                percentage += Time.deltaTime / timeToFullyRegenGuard;
                if (percentage > 1) percentage = 1;
                posture = (percentage * initialPosture);
                print("Regenerating Posture  " + posture);
            }
            if (!canRegen)
            {
                float tempTimer = Time.timeSinceLevelLoad;
                if (tempTimer > timer) canRegen = true;
            }
        }
        public void DamagePosture(Transform attacker, int damage, CurrentActionsWhenDamaged action)
        {
            float temp = posture;
            temp -= damage;
            if (temp <= 0) temp = 0;
            posture = temp;
            if (posture <= 0)
            {
                health.Stunned();
                return;
            }
            else
            {
                NoPostureBarDepletedAction(action);
                RestartRegenAction();
            }
        }

        void NoPostureBarDepletedAction(CurrentActionsWhenDamaged action)
        {
            switch (action)
            {
                case CurrentActionsWhenDamaged.NormalAttacking:
                    break;
                case CurrentActionsWhenDamaged.UnblockableAttacking:
                    health.Stagger();
                    break;
                case CurrentActionsWhenDamaged.Parrying:
                    health.Stagger();
                    break;
                case CurrentActionsWhenDamaged.NoAttackAction:
                    health.Block();
                    break;
                default:
                    break;
            }
        }

        public void RefillPosture()
        {
            OnGuardBarRecoveredAfterStun();
            posture = initialPosture;
        }


        void RestartRegenAction()
        {
            canRegen = false;
            timer = Time.timeSinceLevelLoad + timerThreshold;
        }

        public float GetPosture()
        {
            return posture;
        }

        public float GetInitialPosture()
        {
            return initialPosture;
        }
    }

}

