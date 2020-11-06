using PSmash.Movement;
using System;
using System.Collections;
using UnityEngine;
using PSmash.Resources;
using Spine.Unity;
using Spine;
using PSmash.Core;

namespace PSmash.Combat
{
    public class PlayerFighterV2 : MonoBehaviour
    {
        [SpineBone(dataField: "skeletonRenderer")]
        [SerializeField] public string boneName;
        [Header("General Info")]
        [SerializeField] SecondaryWeaponsList weapons;
        [SerializeField] GameObject subWeapon = null;
        [SerializeField] LayerMask whatIsAttackable;
        [SerializeField] LayerMask whatIsEnemy;

        [Header("Combo Attack")]
        [SerializeField] Transform attackTransform = null;
        [SerializeField] int[] comboAttackDamages;
        [SerializeField] Vector2 comboAttackArea = new Vector2(1.5f, 1.5f);
        [SerializeField] AudioClip[] attackSounds = null;
        [SerializeField] AudioClip finisherSound = null;
        [SerializeField] Transform steamTransform = null;
        [SerializeField] Transform shockwaveTransform = null;

        [Header("AirSmash Attack")]
        [SerializeField] AudioClip splashAttackSound = null;
        [SerializeField] Vector2 splashAttackAreaSize = new Vector2(0, 0);
        [SerializeField] int splashDamage = 20;

        [Header("Guard")]
        [SerializeField] Collider2D guardTrigger = null;
        [SerializeField] Collider2D parryTrigger = null;
        [SerializeField] AudioClip parrySound = null;
        [SerializeField] float parryTime = 2f;

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

        Animator animator;
        AudioSource audioSource;
        PlayerMovementV2 movement;
        Coroutine coroutine;
        PlayerHealth health;
        SkeletonMecanim mecanim;
        TimeManager timeManager;
        Transform targetTransform;

        Bone bone;
        bool canContinueCombo = false;
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
            movement = GetComponent<PlayerMovementV2>();
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
            DoFinisherMove();
            return;
            if (IsFinishingAnEnemy()) return;

            if (IsEnemyStunned())
            {
                print("Enemy is Stunned");
                isAttacking = true;
                DoFinisherMove();
            }
            if (!movement.IsGrounded() && animator.GetInteger("Attack") == 0 && yInput <-0.5f /*&& Mathf.Abs(movement.x) < 0.2f*/)
            {
                //SplashDownAttack
                print("SplashAttack");
                animator.SetInteger("Attack", 50);
                isAttacking = true;
                StartCoroutine(IsAttackingAnimationStatus("Attack"));
                return;
            }
            else if (animator.GetInteger("Attack") == 0 && isButtonPressed)
            {
                //Combo Initial Attack
                movement.SetVelocityToCero();
                //print("ComboAttack");
                animator.SetInteger("Attack", 1);
                isAttacking = true;
                StartCoroutine(IsAttackingAnimationStatus("Attack"));
                return;
            }
            else if(animator.GetInteger("Attack")!=0 && canContinueCombo)
            {
                //Combo Attack Continuity
                //print("ComboAttackContinuity");
                animator.SetInteger("Attack", animator.GetInteger("Attack") + 1);
                canContinueCombo = false;
                return;
            }
        }        
        
        //Waiting for Action to Finish
        IEnumerator IsAttackingAnimationStatus(string action)
        {
            if (action == "Guard")
            {
                guardTrigger.enabled = true;
            }
            movement.CanFlip = false;
            while (animator.GetInteger(action) != 100 && !health.IsDamaged())
            {
                yield return new WaitForEndOfFrame();
                if (health.IsDamaged()) break;
            }
            // print("Player Attack Finished");
            animator.SetInteger(action, 0);
            isGuarding = false;
            heavyAttacking = false;
            movement.CanFlip = true;
            isAttacking = false;
            guardTrigger.enabled = false;
            if (IsFinishingAnEnemy()) isFinishinAnEnemy = false;
            print("Attack Finished");
        }


        //Waiting for Action to Finish
        IEnumerator IsChargingAttackAnimationStatus()
        {
            StopCoroutine(coroutine);
            while (animator.GetInteger("Attack") != 100)
            {
                if (isChargeAttackReady && !isToolButtonPressed)
                {
                    //ReleaseChargeAttack
                    isAttacking = true;
                    animator.SetInteger("Attack", 80);
                    StartCoroutine(IsAttackingAnimationStatus("Attack"));
                    yield break;
                }
                if (!isToolButtonPressed && !isChargeAttackReady)
                {
                    //StopCharging the Attack
                    animator.SetInteger("Attack", 95);
                }
                //Debug.Log("Waiting for Charge Attack To Finish");
                yield return new WaitForEndOfFrame();
            }
            movement.CanFlip = true;
            animator.SetInteger("Attack", 0);
            isChargeAttackReady = false;
            isAttacking = false;
        }
        public void ToolAttack(bool isButtonPressed)
        {
            isToolButtonPressed = isButtonPressed;
            print(isButtonPressed);
            print(isAttacking + "  " + isChargingChargeAttack + "  " + isChargeAttackReady);
            if (isAttacking && !isChargingChargeAttack && !isChargeAttackReady) SetActionAfterAttackEnds(isButtonPressed);

            if (!isAttacking && animator.GetInteger("Attack") == 0 && isButtonPressed)
            {
                isAttacking = true;
                animator.SetInteger("Attack", 60);
                coroutine = StartCoroutine(IsAttackingAnimationStatus("Attack"));
                return;
            }
        }

        private void SetActionAfterAttackEnds(bool isButtonPressed)
        {
            //This method will be used only after the attackin animation is playing
            //This is to check the status of the button
            //61 will be the value to pass directly to the charge attack animation
            //70 will be the value to end the animation
            print("Setting after attack action  " + isButtonPressed);
            if (!isButtonPressed)
            {
                animator.SetInteger("Attack", 70);
                return;
            }
            if (isButtonPressed)
            {
                animator.SetInteger("Attack", 61);
                return;
            }
        }

        bool IsEnemyStunned()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 1), transform.right, 2, whatIsEnemy);
            if (!hit) return false;
            return hit.transform.GetComponent<EnemyHealth>().IsStunned();
        }

        void DoFinisherMove()
        {
            isFinishinAnEnemy = true;
            print("Player is doing Finisher Move");
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 1), transform.right, 2, whatIsEnemy);
            //Move Player to a specific position relative to the enemy
            if(transform.position.x - hit.transform.position.x > 0)
            {
                //Player is at right side of enemy
                GetComponent<Rigidbody2D>().MovePosition(hit.transform.position + new Vector3(1, 0, 0));
            }
            else
            {
                //Player is at left side of enemy
                GetComponent<Rigidbody2D>().MovePosition(hit.transform.position + new Vector3(-1, 0, 0));
            }
            print("Player positioned relative to the enemy");
            targetTransform = hit.transform;
            targetTransform.GetComponent<EnemyHealth>().StartFinisherAnimation();
            animator.SetInteger("Attack", 80);
            StartCoroutine(IsAttackingAnimationStatus("Attack"));
        }


        //Anim Event
        void StartSteamParticles()
        {
            steamTransform.GetComponent<ParticleSystem>().Play();
        }

        //Anim Event
        void StopSteamParticles()
        {
            steamTransform.GetComponent<ParticleSystem>().Stop();
        }

        //Anim Event
        void Shockwave()
        {
            shockwaveTransform.GetComponent<ParticleSystem>().Play();
        }

        //Anim Event
        void PushTargetAway()
        {
            targetTransform.GetComponent<EnemyHealth>().BeingPushedAway(transform.position);
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
            StartCoroutine(FinisherTimer());
        }
        IEnumerator FinisherTimer()
        {
            print("Start to wait");
            timeManager.SlowTime();
            yield return new WaitForSecondsRealtime(2.5f);
            timeManager.SpeedUpTime();

            print("Ended waiting");
        }

        public void Guard(bool isGuardButtonPressed)
        {
            this.isGuardButtonPressed = isGuardButtonPressed;
            if (this.isGuardButtonPressed)
            {
                Guard();
            }
            else
            {
                isGuarding = false;
                animator.SetInteger("Guard", 50);
            }
        }

        public void Guard()
        {
            isGuarding = true;
            animator.SetInteger("Guard", 1);
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(EnablingParryTrigger());
            StartCoroutine(IsAttackingAnimationStatus("Guard"));
        }

        IEnumerator EnablingParryTrigger()
        {
            float timer = 0;
            parryTrigger.enabled = true;
            while (timer < parryTime && isGuarding ==true)
            {
                timer += Time.deltaTime/0.7f;
                yield return new WaitForEndOfFrame();
            }
            parryTrigger.enabled = false;
            coroutine = null;
        }

        public void StartParry()
        {
            animator.SetTrigger("Parry");
            audioSource.PlayOneShot(parrySound);
            isAttacking = true;
            movement.CanFlip = false;
            guardTrigger.enabled = false;
        }

        public void EndParry()
        {
            isAttacking = false;
            movement.CanFlip = true;
            guardTrigger.enabled = false;
        }

        private void SendDamage(Transform attackOriginPosition, Vector2 attackArea, int damage)
        {
            Collider2D[] colls = Physics2D.OverlapBoxAll(attackOriginPosition.position, attackArea, 0, whatIsAttackable);
            if (colls.Length == 0) return;
            foreach (Collider2D coll in colls)
            {
                IDamagable target = coll.GetComponent<IDamagable>();
                if (target == null) continue;
                if (isFinishinAnEnemy)
                {
                    damage *= 10;
                    print("Enemy being Finished");
                }
                target.TakeDamage(transform, damage);
            }
        }

        //Anim Event
        void SetNewPosition()
        {
            Vector3 newPosition = bone.GetWorldPosition(mecanim.transform);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 0.5f), transform.right, Vector3.Distance(transform.position, newPosition), whatIsAttackable);

            if (hit)
            {
                if (transform.right.x > 0)
                {
                    newPosition = new Vector3(newPosition.x - 0.1f, newPosition.y);
                }
                else
                {
                    newPosition = new Vector3(newPosition.x + 0.1f, newPosition.y);
                }
            }
            else
            {
                newPosition = new Vector3(newPosition.x, newPosition.y);
            }
            transform.position = newPosition;
        }

        //AnimEvent
        void StartChargeAttack()
        {
            isAttacking = false;
            isChargingChargeAttack = true;
            StartCoroutine(IsChargingAttackAnimationStatus());
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
                canContinueCombo = true;
            }
            else canContinueCombo = false;
        }

        //Anim Event
        void SetChargeAttackToReady()
        {
            isChargeAttackReady = true;
            Debug.Log("Charge Attack is ready");
        }
        //Anim Event
        void ComboAttackSound(int sound)
        {
            audioSource.PlayOneShot(attackSounds[sound-1]);
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
    }
}

