using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{
    public class EnemyPosture : MonoBehaviour
    {
        [SerializeField] float posture = 50;
        [SerializeField] float timeToCanRegen = 1f;
        [SerializeField] float timeToFullyRegenGuard = 60f;

        EnemyHealth health;

        float timerCanRegen = 0;
        float initialPosture;

        private void Awake()
        {
            health = GetComponent<EnemyHealth>();
            initialPosture = posture;
        }

        private void Update()
        {
            if (health.IsDead() || health.IsStunned())
            {
                posture = 0;
                return;
            }
            if( posture < initialPosture && timerCanRegen > timeToCanRegen)
            {
                posture = RegenPosture();
            }
            UpdateTimers();
        }

        private float RegenPosture()
        {
            float percentage = (float)this.posture / (float)initialPosture;
            percentage += Time.deltaTime / timeToFullyRegenGuard;
            if (percentage > 1) percentage = 1;
            percentage = percentage * initialPosture;
            return percentage;
        }

        void UpdateTimers()
        {
            timerCanRegen += Time.deltaTime;
        }

        public void DamagePosture(int damage, DamagedActionsList action)
        {
            posture = PostureDamaged(damage);
            if (posture <= 0)
            {
                health.Stunned(damage);
                return;
            }
            else
            {
                PostureBarAliveAction(damage,action);
                timerCanRegen = 0;
            }
        }

        private float PostureDamaged(int damage)
        {
            float posture = this.posture;
            posture -= damage;
            if (posture <= 0) posture = 0;
            return posture;
        }

        void PostureBarAliveAction(int damage,DamagedActionsList action)
        {
            switch (action)
            {
                case DamagedActionsList.NormalAttacking:
                    health.ContinueCurrentAction(damage);
                    break;
                case DamagedActionsList.UnblockableAttacking:
                    if(health.GetAccumulatedDamage() > health.GetDamageThreshold())
                    {
                        health.Stagger(damage);
                    }
                    else
                    {
                        health.ContinueCurrentAction(damage);
                    }
                    break;
                case DamagedActionsList.Parrying:
                    health.Stagger(damage);
                    break;
                case DamagedActionsList.Blocking:
                    if (UnityEngine.Random.Range(0, 20) < health.GetCounterProbability())
                    {
                        health.ContinueCurrentAction(damage);
                        print("Will Attack finishing guarding");
                    }
                    else
                    {
                        health.Block(damage);
                    }
                    break;
                case DamagedActionsList.NoAction:
                    health.Block(damage);
                    break;
                default:
                    break;
            }
        }

        public float GetPosture()
        {
            return posture;
        }

        public float GetInitialPosture()
        {
            return initialPosture;
        }

        //UnityEvent (EnemyHealth)
        public void FullyRegenPosture()
        {
            posture = initialPosture;
        }
    }

}

