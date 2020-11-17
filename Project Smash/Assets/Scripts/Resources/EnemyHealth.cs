﻿using System;
using System.Collections;
using UnityEngine;
using PSmash.Core;
using PSmash.Combat;

namespace PSmash.Resources
{
    public class EnemyHealth : Health, IAction
    {
        [Header("TestMode")]
        [SerializeField] bool invulnerable = true;


        [Header("Extras")]
        [SerializeField] AudioClip damagedSound = null;
        [SerializeField] AudioClip staggeredSound = null;
        [SerializeField] AudioClip stunnedSound = null;
        [SerializeField] AudioClip deadSound = null;
        [SerializeField] AudioClip guardDamageSound = null;
        [SerializeField] GameObject dropItem = null;
        [SerializeField] AudioClip finisherDamageSound = null;

        Coroutine coroutine;
        Animator animator;
        EnemyPosture posture;
        bool isStunned = false;
        bool isStaggered = false;
        bool cr_Running = false;
        bool isDamaged = false;
        bool isBlocking = false;
        bool isBeingFinished = false;
        float impulseVelocity = 0;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            posture = GetComponent<EnemyPosture>();
        }

        // Start is called before the first frame update
        void Start()
        {
            initialHealth = health;
        }

        public override void TakeDamage(Transform attacker, int damage)
        {
            print(gameObject.name + " damage received");
            EnemyAttack attack = GetComponent<EnemyAttack>();
            if (isDead) return;
            //Damage while stunned
            //For now this must not enter since the player will kill it instantly
            //Further in the game the enemy could be attacked without being locked on
            if (isStunned)
            {
                Debug.LogWarning(gameObject.name + "  was damaged being stunned");
                return;
            }
            //Damaged by Parry
            else if (attack.IsParried)
            {
                attack.IsParried = false;
                posture.DamagePosture(attacker, damage,
                                      EnemyPosture.CurrentActionsWhenDamaged.Parrying);
            }
            //Damage while Normal Attacking
            else if (attack.GetIsNormalAttacking())
            {
                print(gameObject.name + "  is damaged while attacking");
                posture.DamagePosture(attacker, damage, 
                                      EnemyPosture.CurrentActionsWhenDamaged.NormalAttacking);
                DamageHealthBar(attacker, damage);
                audioSource.PlayOneShot(damagedSound);
                if (health <= 0)
                {
                    Dead();
                    return;
                }
            }
            //Damaged while performing Unblockable Attack
            else if (attack.GetIsUnblockableAttacking())
            {
                print(gameObject.name + "  is damaged while unblockable attacking");
                posture.DamagePosture(attacker, damage, 
                                      EnemyPosture.CurrentActionsWhenDamaged.UnblockableAttacking);
                DamageHealthBar(attacker, damage);
                audioSource.PlayOneShot(damagedSound);
                if (health <= 0)
                {
                    Dead();
                    return;
                }
            }
            else
            {
                if (posture.GetPosture() >0)
                {
                    print(gameObject.name + "  Is Damaged while not attacking");

                    posture.DamagePosture(attacker, damage, EnemyPosture.CurrentActionsWhenDamaged.NoAttackAction);
                }
                else
                {
                    Debug.LogWarning(gameObject.name + "  was damaged while doing nothing");
                    DamageHealthBar(attacker, damage);
                    audioSource.PlayOneShot(damagedSound);
                    if (health <= 0)
                    {
                        Dead();
                        return;
                    }
                }
            }
        }

        private void DamageHealthBar(Transform attacker, int damage)
        {
            if (invulnerable) return;
            int temp = health;
            temp -= damage;
            if (temp <= 0) temp = 0;
            health = temp;
        }

        public void Block()
        {
            print(gameObject + "  is Blocking");
            isBlocking = true;
            GetComponent<ActionScheduler>().StartAction(this);
            animator.SetInteger("guard", 1);
            audioSource.PlayOneShot(guardDamageSound);
            if (cr_Running) StopCoroutine(coroutine);
            coroutine = StartCoroutine(AnimationStateCheck("guard"));
        }

        public void Stagger()
        {
            if (isDead)
            {
                Debug.LogWarning("Wants to staggered when is already dead");
            }
            print(gameObject + "  is Staggered");
            GetComponent<ActionScheduler>().StartAction(this);
            animator.SetInteger("guard", 30);
            audioSource.PlayOneShot(staggeredSound);
            isStaggered = true;
            if (cr_Running) StopCoroutine(coroutine);
            coroutine = StartCoroutine(AnimationStateCheck("guard"));
        }
        public void Stunned()
        {
            if (isDead)
            {
                Debug.LogWarning("Wants to staggered when is already dead");
            }
            print(gameObject + "  is Stunned");

            GetComponent<ActionScheduler>().StartAction(this);
            animator.SetInteger("guard", 30);

            audioSource.PlayOneShot(stunnedSound);
            isStunned = true;
            if (cr_Running) StopCoroutine(coroutine);
            coroutine = StartCoroutine(AnimationStateCheck("guard"));
        }
        IEnumerator AnimationStateCheck(String action)
        {
            cr_Running = true;
            while (animator.GetInteger(action) != 100)
            {
                yield return new WaitForEndOfFrame();
            }
            isStaggered = false;
            cr_Running = false;
            isBlocking = false;
            if (isStunned)
            {
                print("Refilling Posture Bar");
                isStunned = false;

                posture.RefillPosture();
            }
        } 
        public void StartFinisherAnimation()
        {
            StopAllCoroutines();
            animator.SetInteger("finisher", 1);
            GetComponent<ActionScheduler>().StartAction(this);
        }

        public void SpeedUpSound()
        {
            audioSource.pitch = 1;
        }


        public void DeliverFinishingBlow(Vector3 attackerPosition)
        {
            StartCoroutine(FinisherReaction(attackerPosition));
        }
        public IEnumerator FinisherReaction(Vector3 attackerPosition)
        {
            float position = attackerPosition.x - transform.position.x;
            GetComponent<Rigidbody2D>().gravityScale = 1;
            GetComponent<Rigidbody2D>().drag = 2.5f;
            //if (position > 0)
            //{
            //    //Playeris at the right side
            //    GetComponent<Rigidbody2D>().velocity = new Vector2(-28, 14);
            //}
            //else
            //{
            //    GetComponent<Rigidbody2D>().velocity = new Vector2(28, 14);
            //}
            DamageHealthBar(null, initialHealth);
            while (animator.GetInteger("finisher")!= 100)
            {
                yield return null;
            }

            StartCoroutine(GameObjectDied());
        }
        private void Dead()
        {
            isDead = true;
            print("Is Dead");
            //Debug.Break();
            GetComponent<ActionScheduler>().StartAction(this);
            StopAllCoroutines();
            print("Updated values to dead ones");
            animator.SetInteger("attack", 0);
            animator.SetInteger("guard", 0);
            animator.SetInteger("finisher", 0);
            animator.SetInteger("isDead", 100);
            StartCoroutine(GameObjectDied());
        }
        IEnumerator GameObjectDied()
        {
            gameObject.layer = LayerMask.NameToLayer("DyingEnemies");
            GetComponent<Rigidbody2D>().drag = 2;
            audioSource.PlayOneShot(deadSound,0.4f);
            yield return new WaitForSeconds(2);
            Destroy(transform.parent.gameObject);
        }

        public override void Kill(Transform attacker)
        {
            DamageHealthBar(attacker, initialHealth);
        }


        public bool IsDamaged
        {
            get
            {
                return isDamaged;
            }
            set
            {
                isDamaged = value;
            }
        }

        public void Cancel()
        {
            //Stop whatever damage is being done
            //print("Damage Canceled");
        }

        public float GetHealthValue()
        {
            return health;
        }

        public float GetInitialHealthValue()
        {
            return initialHealth;
        }

        public bool IsDead()
        {
            return isDead;
        }

        public bool IsStaggered()
        {
            return isStaggered;
        }

        public bool IsBlocking()
        {
            return isBlocking;
        }

        public bool IsStunned()
        {
            return isStunned;
        }

        public bool IsBeingFinished()
        {
            return isBeingFinished;
        }

        //Event used with the guarding animation to reset the variable value to false
        //and allow the other scripts to pass this flag in the Update Loop
        //Anim Event
        void ResetIsInterruptedState()
        {
            isStunned = false;
        }

        //Anim Event
        void DropItem()
        {
            Instantiate(dropItem, transform.position + new Vector3(0,1), Quaternion.identity);
        }
    }
}


