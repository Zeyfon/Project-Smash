using Cinemachine;
using PSmash.Movement;
using System;
using System.Collections;
using UnityEngine;

namespace PSmash.Combat
{
    public class PlayerFighterV2 : MonoBehaviour
    {
        [Header("General Info")]
        [SerializeField] SecondaryWeaponsList weapons;
        [SerializeField] GameObject subWeapon = null;
        [SerializeField] LayerMask whatIsAttackable;

        [Header("Molotov Bomb")]
        [SerializeField] GameObject bomb;
        [SerializeField] float thrownSpeed =5;
        [SerializeField] Vector2 weaponOriginPosition;

        [Header("Combo Attack")]
        [SerializeField] Transform attackTransform = null;
        [SerializeField] int[] comboAttackDamages;
        [SerializeField] float[] comboImpulses;
        [SerializeField] Vector2 comboAttackArea = new Vector2(1.5f,1.5f);
        [SerializeField] AudioClip[] attackSounds = null;

        [Header("AirSmash Attack")]
        [SerializeField] AudioClip splashAttackSound = null;
        [SerializeField] Vector2 splashAttackAreaSize = new Vector2(0, 0);
        [SerializeField] int splashDamage = 20;

        [Header("Parry")]
        [SerializeField] AudioClip parrySound = null;

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
        PlayerMovementV2 playerMovement;
        Coroutine coroutine;
        bool canContinueCombo = false;
        bool isAttacking = false;
        bool isToolButtonPressed = false;
        bool isChargingChargeAttack = false;
        bool isChargeAttackReady = false;
        bool heavyAttacking = false;
        bool isGuarding = false;

        void Awake()
        {
            playerMovement = GetComponent<PlayerMovementV2>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        public void MainAttack(bool isButtonPressed, float yInput)
        {
            //Debug.Log("Wants To Attack");
            if(!playerMovement.IsGrounded && animator.GetInteger("Attack") == 0 && yInput <-0.5f /*&& Mathf.Abs(movement.x) < 0.2f*/)
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
                print("ComboAttack");
                animator.SetInteger("Attack", 1);
                isAttacking = true;
                StartCoroutine(IsAttackingAnimationStatus("Attack"));
                return;
            }
            else if(animator.GetInteger("Attack")!=0 && canContinueCombo)
            {
                //Combo Attack Continuity
                print("ComboAttackContinuity");
                animator.SetInteger("Attack", animator.GetInteger("Attack") + 1);
                canContinueCombo = false;
                return;
            }
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

        public void Guard(bool isGuardButtonPressed)
        {
            if (isGuardButtonPressed)
            {
                isGuarding = true;
                animator.SetInteger("Guard", 1);
                StartCoroutine(IsAttackingAnimationStatus("Guard"));
            }
            else
            {
                animator.SetInteger("Guard", 50);
            }
        }

        private void SendDamage(Transform origin, Vector2 attackArea, int damage)
        {
            Collider2D[] colls = Physics2D.OverlapBoxAll(origin.position, attackArea, 0, whatIsAttackable);
            if (colls.Length == 0) return;
            foreach (Collider2D coll in colls)
            {
                if (!coll.GetComponent<EnemyHealth>())
                {
                    Debug.Log(coll.gameObject);
                    coll.SendMessage("Hit");
                    return;
                }
                coll.GetComponent<EnemyHealth>().DamageTaken(transform, damage);
            }
        }

        //Waiting for Action to Finish
        IEnumerator IsAttackingAnimationStatus(string action)
        {
            playerMovement.CanFlip = false;
            while (animator.GetInteger(action) != 100 )
            {
                yield return new WaitForEndOfFrame();
            }
            animator.SetInteger(action, 0);
            isGuarding = false;
            heavyAttacking = false;
            playerMovement.CanFlip = true;
            isAttacking = false;
        }

        //Waiting for Action to Finish
        IEnumerator IsChargingAttackAnimationStatus()
        {
            StopCoroutine(coroutine);
            while (animator.GetInteger("Attack") != 100)
            {
                if(isChargeAttackReady && !isToolButtonPressed)
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
            playerMovement.CanFlip = true;
            animator.SetInteger("Attack", 0);
            isChargeAttackReady = false;
            isAttacking = false;
        }

        public bool IsChargeAttackReady()
        {
            return isChargeAttackReady;
        }

        public bool IsAttacking
        {
            get
            {
                return isAttacking;
            }
        }
        public bool IsGuarding
        {
            get
            {
                return isGuarding;
            }
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

        //Anim Event
        void SpawnBomb()
        {
            GameObject bombClone = Instantiate(bomb, attackTransform.position, Quaternion.identity);
            bombClone.GetComponent<Rigidbody2D>().velocity = new Vector2(thrownSpeed * transform.right.x, 0);
        }

        //Anim Event
        void ComboAttackImpulses(int comboAttackNumber)
        {
            playerMovement.AddImpulse(comboImpulses[comboAttackNumber - 1]);
        }
        //AnimEvent
        void ToolAttackImpulse()
        {
            playerMovement.AddImpulse(heavyAttackImpulse);
        }

        //AnimEvent
        void ToolAttackSound()
        {
            audioSource.PlayOneShot(currentToolSound);
        }
        //AnimEvent
        void ChargeAttackImpulse()
        {
            playerMovement.AddImpulse(chargeAttackImpulse);

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

        //Anim Event
        void ParrySound()
        {
            audioSource.PlayOneShot(parrySound);
        }

        //Anim Event
        void ForwardAttackSound()
        {
            audioSource.PlayOneShot(forwardAttackSound);

        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireCube(attackTransform.position, comboAttackArea);
        }
    }
}

