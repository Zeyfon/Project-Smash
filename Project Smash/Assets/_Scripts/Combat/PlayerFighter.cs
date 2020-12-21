using PSmash.Attributes;
using PSmash.Movement;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using PSmash.Combat.Weapons;

namespace PSmash.Combat
{
    public class PlayerFighter : MonoBehaviour
    {
        [SpineBone(dataField: "skeletonRenderer")]
        [SerializeField] public string boneName;
        [Header("General Info")]
        [SerializeField] SecondaryWeaponsList weapons;
        [SerializeField] GameObject subWeapon = null;
        [SerializeField] LayerMask whatIsDamagable;
        [SerializeField] LayerMask whatIsEnemy;

        [Header("Combo Attack")]
        [SerializeField] Transform attackTransform = null;
        [SerializeField] int[] comboAttackDamages;
        [SerializeField] Vector2 comboAttackArea = new Vector2(1.5f, 1.5f);
        [SerializeField] AudioClip attackSound1 = null;
        [SerializeField] AudioClip attackSound2 = null;

        [Header("Finishing Move")]
        [SerializeField] int finisherDamage = 100;
        [SerializeField] AudioClip finisherSound = null;
        [SerializeField] Transform steamTransform = null;
        [SerializeField]  GameObject finisherEffects = null;

        [Header("Throw Attack")]
        [SerializeField] GameObject dagger = null;
        public int currentItemQuantity = 3;


        [Header("AirSmash Attack")]
        [SerializeField] AudioClip splashAttackSound = null;
        [SerializeField] Vector2 splashAttackAreaSize = new Vector2(0, 0);
        [SerializeField] int splashDamage = 20;

        [Header("Guard")]
        [SerializeField] PlayerGuard guard = null;
        [SerializeField] Collider2D guardTrigger = null;
        [SerializeField] AudioClip guardFootstepSound = null;
        [SerializeField] float canParryTime = 2f;

        [Header("Forward Attack")]
        [SerializeField] AudioClip forwardAttackSound = null;

        [Header("Damages")]
        [SerializeField] int hikingSpikeDamage = 20;
        [SerializeField] int heavyAttackDamage = 25;
        [SerializeField] int chargeAttackDamage = 35;

        [SerializeField] float heavyAttackImpulse = 25;
        [SerializeField] float chargeAttackImpulse = 30;

        [Header("Sounds")]
        [SerializeField] AudioClip currentToolSound=null;
        public event Action AirSmashAttackEffect;

        public delegate void ThrowItem(int quantity);
        public event ThrowItem onItemThrown;

        Animator animator;
        AudioSource audioSource;
        PlayerMovement movement;
        Coroutine coroutine;
        PlayerHealth health;
        SkeletonMecanim mecanim;
        TimeManager timeManager;
        Transform targetTransform;

        Bone bone;
        bool isComboWindowActive = false;
        bool isAttacking = false;
        bool isToolButtonPressed = false;
        bool isChargingChargeAttack = false;
        bool isChargeAttackReady = false;
        bool heavyAttacking = false;
        bool isGuarding = false;
        bool isGuardButtonPressed = false;
        bool isFinishinAnEnemy = false;

        void Awake()
        {
            health = GetComponent<PlayerHealth>();
            movement = GetComponent<PlayerMovement>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            mecanim = GetComponent<SkeletonMecanim>();
            timeManager = GameObject.FindObjectOfType<TimeManager>();
        }

        private void Start()
        {
            bone = GetComponent<SkeletonRenderer>().skeleton.FindBone(boneName);
        }

        public void MainAttack(bool isButtonPressed, float yInput)
        {

            if (IsFinishingAnEnemy()) return;
            if (!movement.IsGrounded() && animator.GetInteger("Attack") == 0 && yInput <-0.5f)
            {
                //print("Splash Attack");
                isAttacking = true;
                StartCoroutine(RunThisAnimation("Attack", 50));
                return;
            }
            else if (animator.GetInteger("Attack") == 0 && isButtonPressed)
            {
                //print("Main Combo Attack");
                movement.SetVelocityToCero();
                isAttacking = true;
                StartCoroutine(RunThisAnimation("Attack",1));
                
                return;
            }
            else if(animator.GetInteger("Attack")!=0 && isComboWindowActive)
            {
                //print("Combo Attack Continuity");
                isComboWindowActive = false;
                animator.SetInteger("Attack", animator.GetInteger("Attack") + 1);
                return;
            }
        }        

        public bool ToolAttack()
        {
            if (IsFinishingAnEnemy()) return true;
            if (IsEnemyStunned())
            {
                isAttacking = true;
                StartCoroutine(DoFinisherMove());
                return true;
            }
            return false;
        }

        bool IsEnemyStunned()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 1), transform.right, 2, whatIsEnemy);
            if (!hit) return false;
            targetTransform = hit.transform;
            return hit.transform.GetComponent<EnemyHealth>().IsStunned();
        }

        IEnumerator DoFinisherMove()
        {
            SetEnemyStateToFinisher();
            isFinishinAnEnemy = true;
            movement.StopMovement();
            gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
            //print("Player is doing Finisher Move");
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 1), transform.right, 2, whatIsEnemy);
            if (transform.position.x - hit.transform.position.x > 0)
            {
                //Player is at right side of enemy
                GetComponent<Rigidbody2D>().MovePosition(hit.transform.position + new Vector3(1, 0, 0));
            }
            else
            {
                //Player is at left side of enemy
                GetComponent<Rigidbody2D>().MovePosition(hit.transform.position + new Vector3(-1, 0, 0));
            }
            targetTransform = hit.transform;
            StartCoroutine(StartPlayerAndTargetFinisherAnimations());
            yield return null;
        }

        private void SetEnemyStateToFinisher()
        {
            targetTransform.GetComponent<EnemyHealth>().SetStateToFinisher();
        }

        IEnumerator StartPlayerAndTargetFinisherAnimations()
        {
            //targetTransform.GetComponent<EnemyHealth>().StopCurrentActions();
            StartCoroutine(RunThisAnimation("Attack",80));
            while (animator.GetInteger("Attack")!= 81)
            {
                yield return null;
            }
            targetTransform.GetComponent<EnemyHealth>().StartFinisherAnimation();
        }

        //Anim Event
        void StartSteam()
        {
            steamTransform.GetComponent<ParticleSystem>().Play();
        }

        //Anim Event
        void StopSteam()
        {
            steamTransform.GetComponent<ParticleSystem>().Stop();
        }

        //Anim Event
        void Shockwave()
        {

            Instantiate(finisherEffects,attackTransform.position,Quaternion.identity);
        }

        //Anim Event
        void FinisherAttack()
        {
            if (targetTransform == null) return;
            targetTransform.GetComponent<EnemyHealth>().DeliverFinishingBlow(transform.position, finisherDamage);
        }

        //Anim Event
        void FinisherSound()
        {
            audioSource.pitch = 0.3f;
            audioSource.PlayOneShot(finisherSound);
            StartCoroutine(SpeedUpAudioSourcePitch());
        }
        IEnumerator SpeedUpAudioSourcePitch()
        {
            float pitch = audioSource.pitch;
            yield return new WaitForSecondsRealtime(0.1f);
            while (pitch < 1)
            {
                pitch += Time.deltaTime;
                if (pitch >= 1) pitch = 1;
                audioSource.pitch = pitch;
                yield return null;
            }
        }
        //Anim Events
        void TimeScaleDown()
        {
            //StartCoroutine(FinisherTimer());
        }
        IEnumerator FinisherTimer()
        {
            print("Start to wait");
            timeManager.SlowTime();
            yield return new WaitForSecondsRealtime(3f);
            timeManager.SpeedUpTime();

            print("Ended waiting");
        }


        public void ThrowItemAttack(bool isButtonPressed)
        {
            isAttacking = true;
            movement.StopMovement();
            StartCoroutine(RunThisAnimation("Attack", 30));
            //print(this.name + "  Throwing item");
        }

        void SpawnItem()
        {
            if (currentItemQuantity <= 0) return;
            GameObject itemClone = Instantiate(dagger, attackTransform.position, Quaternion.identity);
            itemClone.GetComponent<Projectile>().SetData(movement.GetIsLookingRight());
            currentItemQuantity--;
            onItemThrown(currentItemQuantity);
        }

        #region Guard/Parry
        public void Guard(bool isGuardButtonPressed)
        {
            this.isGuardButtonPressed = isGuardButtonPressed;
            //print(this.name + "  wants to guard3");
            if (this.isGuardButtonPressed)
            {
                StartGuard();
            }
            else
            {
                EndGuard();
            }
        }
        void StartGuard()
        {
            isGuarding = true;
            guardTrigger.enabled = true;
            StartCoroutine(StartPartyWindow());
            StartCoroutine(RunThisAnimation("Guard",1));
        }

        private void EndGuard()
        {
            //isGuarding = false;
            animator.SetInteger("Guard", 50);
        }

        IEnumerator StartPartyWindow()
        {
            guard.SetCanParry(true);
            float timer = 0;
            while (timer < canParryTime && isGuarding ==true)
            {
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            guard.SetCanParry(false);
        }

        void SetCanParry(int parryState)
        {
            if (parryState == 1) guard.SetCanParry(true);
            else guard.SetCanParry(false);
        }

        public void StartParry()
        {
            animator.SetTrigger("Parry");
            isAttacking = true;
            movement.CanFlip = false;
            guardTrigger.enabled = false;
            isGuarding = false;
        }

        //AnimEvent
        void GuardFootstepSound()
        {
            audioSource.PlayOneShot(guardFootstepSound);
        }
        #endregion

        //Waiting for Action to Finish
        IEnumerator RunThisAnimation(string action,int value)
        {
            animator.SetInteger(action, value);
            movement.CanFlip = false;
            while (animator.GetInteger(action) != 100 && !health.IsDamaged())
            {
                yield return null;
            }
            // print("Player Attack Finished");
            animator.SetInteger(action, 0);
            isGuarding = false;
            heavyAttacking = false;
            movement.CanFlip = true;
            isAttacking = false;
            guardTrigger.enabled = false;
            isFinishinAnEnemy = false;
            gameObject.layer = LayerMask.NameToLayer("Player");
            //print("Attack Finished");
        }

        //Anim Event
        void LightAttackDamage(int comboAttackNumber)
        {
            SendDamage(attackTransform, comboAttackArea, comboAttackDamages[comboAttackNumber - 1]);
        }

        //Anim Event
        void ToolAttackDamage()
        {
            SendDamage(attackTransform, comboAttackArea, heavyAttackDamage);
        }

        //Anim Event
        void ChargeAttackDamage()
        {
            SendDamage(attackTransform, comboAttackArea, chargeAttackDamage);
        }
        //Anim Event
        void SplashAttack()
        {
            AirSmashAttackEffect();
            audioSource.PlayOneShot(splashAttackSound, 1);
            SendDamage(transform, splashAttackAreaSize, splashDamage);
        }

        private void SendDamage(Transform attackOriginPosition, Vector2 attackArea, int damage)
        {
            //print("Looking to Damage Enemy");
            Collider2D[] colls = Physics2D.OverlapBoxAll(attackOriginPosition.position, attackArea, 0, whatIsDamagable);
            if (colls.Length == 0)
            {
                //print("Nothing was damaged");
                return;
            }
            foreach (Collider2D coll in colls)
            {
                IDamagable target = coll.GetComponent<IDamagable>();
                if (target == null) continue;
                if (isFinishinAnEnemy)
                {
                    damage *= 10;
                    //print("Enemy being Finished");
                }
                target.TakeDamage(transform, damage);
            }
        }

        //AnimEvent
        void ToolAttackImpulse()
        {
            movement.AddImpulse(heavyAttackImpulse);
        }

        //AnimEvent
        void ToolAttackSound()
        {
            audioSource.PlayOneShot(currentToolSound);
        }
        //AnimEvent
        void ChargeAttackImpulse()
        {
            movement.AddImpulse(chargeAttackImpulse);

        }

        //Anim Event
        void IsComboWindowActive(int state)
        {
            if (state == 1)
            {
                //Debug.Log("Combo Window Active");
                isComboWindowActive = true;
            }
            else isComboWindowActive = false;
        }
        //Anim Event
        void LightAttackSound()
        {
            audioSource.pitch = UnityEngine.Random.Range(0.75f, 1.1f);
            audioSource.PlayOneShot(attackSound1);
        }

        public bool IsGuardButtonPressed()
        {
            return isGuardButtonPressed;
        }

        public bool IsChargeAttackReady()
        {
            return isChargeAttackReady;
        }

        public bool IsAttacking()
        {
            return isAttacking;
        }

        public bool IsFinishingAnEnemy()
        {
            return isFinishinAnEnemy;
        }
        public bool IsGuarding()
        {
            return isGuarding;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(attackTransform.position, comboAttackArea);

        }

        ////AnimEvent
        //void StartChargeAttack()
        //{
        //    isAttacking = false;
        //    isChargingChargeAttack = true;
        //    StartCoroutine(IsChargingAttackAnimationStatus());
        //}

        //Anim Event
        //void SetChargeAttackToReady()
        //{
        //    isChargeAttackReady = true;
        //    Debug.Log("Charge Attack is ready");
        //}

        //Waiting for Action to Finish
        //IEnumerator IsChargingAttackAnimationStatus()
        //{
        //    StopCoroutine(coroutine);
        //    while (animator.GetInteger("Attack") != 100)
        //    {
        //        if (isChargeAttackReady && !isToolButtonPressed)
        //        {
        //            //ReleaseChargeAttack
        //            isAttacking = true;
        //            animator.SetInteger("Attack", 80);
        //            StartCoroutine(IsAttackingAnimationStatus("Attack"));
        //            yield break;
        //        }
        //        if (!isToolButtonPressed && !isChargeAttackReady)
        //        {
        //            //StopCharging the Attack
        //            animator.SetInteger("Attack", 95);
        //        }
        //        //Debug.Log("Waiting for Charge Attack To Finish");
        //        yield return new WaitForEndOfFrame();
        //    }
        //    movement.CanFlip = true;
        //    animator.SetInteger("Attack", 0);
        //    isChargeAttackReady = false;
        //    isAttacking = false;
        //}

    }
}

