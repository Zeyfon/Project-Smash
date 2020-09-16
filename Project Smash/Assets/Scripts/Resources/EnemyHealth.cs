using System;
using System.Collections;
using UnityEngine;
using PSmash.Core;

namespace PSmash.Resources
{
    public class EnemyHealth : Health, IAction
    {
        [Header("TestMode")]
        [SerializeField] bool testMode = false;
        [SerializeField] bool hasGuardBar = true;

        [Header("Guard")]
        [SerializeField] int guard = 100;
        [Tooltip("Time will pass between the attack and start regeneration the guard bar")]
        [SerializeField] float timerThreshold = 3;
        [Tooltip("Time it will take to regen the whole bar")]
        [SerializeField] float timeToFullyRegenGuard = 2;

        [Header("Extras")]
        [SerializeField] AudioClip damagedSound = null;
        [SerializeField] AudioClip guardBrokeSound = null;
        [SerializeField] AudioClip deadSound = null;
        [SerializeField] AudioClip guardDamageSound = null;
        [SerializeField] GameObject dropItem = null;

        public event Action OnGuardBarRecoveredAfterStun;

        Coroutine coroutine;
        Animator animator;
        bool isInterrupted = false;
        bool isStunned = false;
        bool isParried = false;
        int initialGuard;
        float impulseVelocity = 0;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        // Start is called before the first frame update
        void Start()
        {
            initialHealth = health;
            initialGuard = guard;
        }

        public override void TakeDamage(Transform attacker, int damage)
        {
            if (isDead) return;
            if (guard>0 && !isStunned)
            {
                DamageGuard(attacker, damage);
            }
            else
            {
                DamageHealth(attacker, damage);
            }
        }

        //Guard damage will be an interruption for the animations being performed by the enemy
        private void DamageGuard(Transform attacker, int damage)
        {
            isInterrupted = true;
            int tempGuard = guard;
            GetComponent<ActionScheduler>().StartAction(this);
            tempGuard -= damage;
            print(tempGuard);
            if (tempGuard <= 0)
            {
                isStunned = true;
                tempGuard = 0;
                animator.SetInteger("guard", 30);
                impulseVelocity = 7;
                audioSource.PlayOneShot(guardBrokeSound);
                print("Guard Broke");
            }
            else
            {
                if (isParried)
                {
                    print("parried");
                    animator.SetInteger("guard", 10);
                }
                else
                {
                    print("Guarded");
                    animator.SetInteger("guard", 1);
                }
                impulseVelocity = 2;
                audioSource.PlayOneShot(guardDamageSound);
            }
            guard = tempGuard;
            GetComponent<PSmash.Movement.EnemyMovement>().ImpulseFromAttack(attacker, impulseVelocity);
            GuardRegeProcess();
        }

        void DamageHealth(Transform attacker, int damage)
        {
            int tempHealth = health;
            if (testMode) return;
            tempHealth -= damage;
            audioSource.PlayOneShot(damagedSound);
            if (tempHealth <= 0)
            {
                tempHealth = 0;
                guard = 0;
                isDead = true;
                StopAllCoroutines();
                StartCoroutine(GameObjectDied());
            }
            else
            {
                impulseVelocity = 2;
            }
            health = tempHealth;
        }

        private void GuardRegeProcess()
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(RegenGuard());
        }

        IEnumerator RegenGuard()
        {    
            yield return WaitingTimeToStartRegen(timerThreshold);
            yield return RegeneratingGuard();
            animator.SetInteger("guard",50);

            if (isStunned) OnGuardBarRecoveredAfterStun();
            isStunned = false;
            while (animator.GetInteger("guard") != 100)
            {
                yield return new WaitForEndOfFrame();
            }
            animator.SetInteger("guard", 0);
            isInterrupted = false;
            coroutine = null;
        }

        IEnumerator WaitingTimeToStartRegen(float timerThreshold)
        {
            float timer = 0;
            while(timer < timerThreshold)
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator RegeneratingGuard()
        {
            float timer = Time.timeSinceLevelLoad;
            float percentage = (float)guard / (float)initialGuard;
            while (percentage < 1)
            {
                //if (timer < Time.timeSinceLevelLoad - 0.3f) canGuardBeDamaged = true;
                percentage += Time.deltaTime / timeToFullyRegenGuard;
                if (percentage > 1) percentage = 1;
                guard = (int)(percentage * initialGuard);
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator GameObjectDied()
        {
            animator.SetTrigger("isDead");
            GetComponent<Rigidbody2D>().gravityScale = 0;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            audioSource.PlayOneShot(deadSound,0.4f);
            yield return new WaitForSeconds(2);
            Destroy(transform.parent.gameObject);
        }

        public override void Kill(Transform attacker)
        {
            DamageHealth(attacker, initialHealth);
        }

        public void Cancel()
        {
            //Stop whatever damage is being done
            //print("Damage Canceled");
        }

        public float GetGuardValue()
        {
            return guard;
        }

        public float GetInitialGuardValue()
        {
            return initialGuard;
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

        public bool IsInterrupted()
        {
            return isInterrupted;
        }

        public bool IsStunned()
        {
            return isStunned;
        }

        public void SetIsParried(bool state)
        {
            isParried = true;
        }

        void ResetIsParryState()
        {
            isParried = false;
            isInterrupted = false;
        }

        //Event used with the guarding animation to reset the variable value to false
        //and allow the other scripts to pass this flag in the Update Loop
        //Anim Event
        void ResetIsInterruptedState()
        {
            isInterrupted = false;
        }

        //Anim Event
        void DropItem()
        {
            Instantiate(dropItem, transform.position + new Vector3(0,1), Quaternion.identity);
        }
    }
}


