using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace PSmash.Attributes
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
        [SerializeField] AudioClip damageWhileStunnedSound = null;
        [SerializeField] AudioClip finisherDamageSound = null;
        [Range(0,100)]
        [SerializeField] int damageHealthPercentage = 20;
        [Range(0, 100)]
        [SerializeField] int percentageDamagePostureToHealthAtStun = 20;
        [Range(0, 100)]
        [SerializeField] float counterAttackProbability = 25;
        [SerializeField] int damageThreshold = 13;
        [SerializeField] UnityEvent onDamaged;
        [SerializeField] UnityEvent onStunned;
        [SerializeField] UnityEvent onStunnedEnded;
        [SerializeField] GameObject dropItem = null;

        Coroutine coroutine;
        Animator animator;
        EnemyPosture posture;
        int accumulatedDamage = 0;
        int consecutiveBlocks = 0;
        bool isStunned = false;
        bool isStaggered = false;
        bool cr_Running = false;
        bool isBlocking = false;
        bool isBeingFinished = false;
        bool isParried = false;
        bool isUnblockableAttacking = false;
        bool isNormalAttacking = false;

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
            ///The enemy reaction to the player attacks will depend mostly on the current action
            ///the enemy is performing (IsStunned, IsNormalAttacking, IsUnblockableAttacking, IsDoingNothing)
            ///based on this the enemy will change to (IsStaggered, IsStunned, IsBlocking, IsParried,
            ///IsContinuingCurrentAction, IsBeingFinished);
            
            //print(gameObject.name + " damage received");
            if (isDead) return;
            onDamaged.Invoke();

            if (isStunned)
            {
                //print(gameObject.name + "  is damaged while being stunned");
                DamageHealth(damage, 100);
                audioSource.PlayOneShot(damageWhileStunnedSound);
            }

            else if (isParried)
            {
                //print(gameObject.name + "  is damaged while being parried");
                isParried = false;
                DamagePosture(damage, DamagedActionsList.Parrying);
            }

            else if (isNormalAttacking)
            {
                //print(gameObject.name + "  is damaged while attacking");
                DamagePosture(damage, DamagedActionsList.NormalAttacking);
            }

            else if (isUnblockableAttacking)
            {
                //print(gameObject.name + "  is damaged while unblockable attacking");
                accumulatedDamage += damage;
                print(accumulatedDamage);
                DamagePosture(damage, DamagedActionsList.UnblockableAttacking);
            }
            else if (isBlocking)
            {
                //print(gameObject.name + " is damagedSound while blocking");
                DamagePosture(damage, DamagedActionsList.Blocking);
            }
            else if (posture.GetPosture() > 0)
            {
                //print(gameObject.name + "  Is Damaged while not attacking");
                DamagePosture(damage, DamagedActionsList.NoAction);
            }
            else
            {
                Debug.LogWarning(gameObject.name + "  was damaged while doing nothing");
                DamageHealth(damage, 100);
                audioSource.PlayOneShot(damagedSound);
            }
        }

        private void DamagePosture(int damage, DamagedActionsList action)
        {
            posture.DamagePosture(damage, action);
        }

        public float GetAccumulatedDamage()
        {
            return accumulatedDamage;
        }

        public void SetAccumulatedDamage(int value)
        {
            accumulatedDamage = value;
        }

        public float GetDamageThreshold()
        {
            return damageThreshold;
        }

        public float GetCounterProbability()
        {
            return counterAttackProbability / 100;
        }


        private void DamageHealth(int damage, int damagePercentage)
        {
            damage = damage * damagePercentage / 100;
            SubstractDamageFromHealth(damage);
            if (health <= 0)
            {
                Dead();
                return;
            }
            return;
        }

        private void SubstractDamageFromHealth(int damage)
        {
            if (invulnerable) return;
            int health = this.health;
            health -= damage;
            if (health <= 0) health = 0;
            this.health = health;
        }



        public void Block(int damage)
        {


            consecutiveBlocks++;
            //print("BlockedAttack " + consecutiveBlocks);
            if (isDead)
            {
                Debug.LogWarning("Wants to block when is already dead");
            }
            isBlocking = true;
            DamageHealth(damage, damageHealthPercentage);
            audioSource.PlayOneShot(guardDamageSound);
            RunReactionAnimation(1);
        }

        public void Stagger(int damage)
        {
            if (isDead)
            {
                Debug.LogWarning("Wants to staggered when is already dead");
            }
            isStaggered = true;
            DamageHealth(damage, damageHealthPercentage);
            audioSource.PlayOneShot(staggeredSound);
            RunReactionAnimation(30);
        }
        public void Stunned(int damage)
        {
            if (isDead)
            {
                Debug.LogWarning("Wants to be stunned when is already dead");
            }
            isStunned = true;
            DamageHealth(damage, 100);
            audioSource.PlayOneShot(stunnedSound);
            RunReactionAnimation(30);
            onStunned.Invoke();
        }

        public void ContinueCurrentAction(int damage)
        {
            DamageHealth(damage, 100);
            audioSource.PlayOneShot(damagedSound);
        }

        private void RunReactionAnimation(int animValue)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            animator.SetInteger("guard", animValue);
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
                isStunned = false;
                onStunnedEnded.Invoke();
            }
        } 

        public void StartFinisherAnimation()
        {
            animator.Play("Finisher");
        }


        public void StopCurrentActions()
        {
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
            float x = 15;
            float y = 7;
            float position = attackerPosition.x - transform.position.x;
            GetComponent<Rigidbody2D>().gravityScale = 1;
            GetComponent<Rigidbody2D>().drag = 2.5f;
            if (position > 0)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(-x, y);
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(x, y);
            }
            SubstractDamageFromHealth(initialHealth);
            yield return null;
        }
        private void Dead()
        {
            isDead = true;
            // print("Is Dead");
            GetComponent<ActionScheduler>().StartAction(this);
            gameObject.layer = LayerMask.NameToLayer("EnemiesGhost");
            GetComponent<Rigidbody2D>().drag = 2;
            audioSource.PlayOneShot(deadSound, 0.4f);
            StopAllCoroutines();
            animator.SetInteger("attack", 0);
            animator.SetInteger("guard", 0);
            animator.SetInteger("isDead", 100);
        }

        void FinisherDead()
        {
            isDead = true;
            DamageHealth(1000000, 100);
        }

        public override void Kill(Transform attacker)
        {
            SubstractDamageFromHealth(initialHealth);
        }

        public void Cancel()
        {
            //Stop whatever damage is being done
            //print("Damage Canceled");
        }

        #region ExternalUse
        public float GetHealthValue()
        {
            return health;
        }

        public float GetInitialHealthValue()
        {
            return initialHealth;
        }

        public int ConsecutiveBlocks
        {
            set
            {
                consecutiveBlocks = value;
            }
            get
            {
                return consecutiveBlocks;
            }
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

        #endregion

        //Set in Enemy Atttack Srcript
        public void SetIsParried(bool state)
        {
            isParried = state;
        }

        //Set in Enemy Atttack Srcript
        public void SetIsNormalAttacking(bool state)
        {
            isNormalAttacking = state;
        }

        //Set in Enemy Atttack Srcript
        public void SetIsUnblockableAttacking(bool state)
        {
            isUnblockableAttacking = state;
        }

        //Anim Event
        void DropItem()
        {
            Instantiate(dropItem, transform.position + new Vector3(0,1), Quaternion.identity);
        }

        //AnimEvent
        void Destroy()
        {
            Destroy(transform.parent.gameObject);
        }
    }
}


