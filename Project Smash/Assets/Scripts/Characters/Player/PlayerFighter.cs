using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Movement;
using System;
using Cinemachine;

namespace PSmash.Combat
{
    public class PlayerFighter : MonoBehaviour
    {
        [Header("General Info")]
        [SerializeField] SecondaryWeaponsList weapons;
        [SerializeField] GameObject subWeapon = null;
        [SerializeField] LayerMask whatIsAttackable;
        [SerializeField] Transform followCamera = null;

        [Header("Molotov Bomb")]
        [SerializeField] GameObject bomb;
        [SerializeField] float thrownSpeed =5;
        [SerializeField] Vector2 weaponOriginPosition;

        [Header("Combo Attack")]
        [SerializeField] Transform attackTransform = null;
        [SerializeField] int comboAttackDamage = 10;
        [SerializeField] float radiusOfAttack = 0.5f;
        [SerializeField] AudioClip[] attackSounds = null;

        [Header("Splash Attack")]
        [SerializeField] int forwardAttackDamage = 15;
        [SerializeField] Vector2 splashAttackSpeed = new Vector2(0, 0);
        [SerializeField] AudioClip splashAttackSound = null;
        [SerializeField] Vector2 splashAttackAreaSize = new Vector2(0, 0);
        [SerializeField] int splashDamage = 20;

        [Header("Parry")]
        [SerializeField] AudioClip parrySound = null;

        [Header("Evade")]
        [SerializeField] AudioClip[] evadeSounds = null;

        [Header("Forward Attack")]
        [SerializeField] AudioClip forwardAttackSound = null;

        [Header("Damages")]
        [SerializeField] int hikingSpikeDamage = 20;

        Animator animator;
        Rigidbody2D rb;
        AudioSource audioSource;
        bool canContinueCombo = false;

        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        public void Attack(bool isPressedAttack,bool isGrounded, float yInput)
        {
            //Debug.Log("Wants To Attack");
            if(!GetComponent<PlayerMovement>().isGrounded && animator.GetInteger("Attack") == 0 && yInput <-0.5f /*&& Mathf.Abs(movement.x) < 0.2f*/)
            {
                //Debug.Log("Smash Attack");
                animator.SetInteger("Attack", 50);
                GravityAdjust();
                return;
            }
            else if (animator.GetInteger("Attack") == 0 && isPressedAttack)
            {
                //Debug.Log("Attack starts");
                animator.SetInteger("Attack", 1);
                return;
            }
            else if(animator.GetInteger("Attack") == 0 && !isPressedAttack)
            {
                animator.SetInteger("Attack", 30);
                StartCoroutine(LookingForEnemy());
                return;
            }
            else if(animator.GetInteger("Attack")!=0 && canContinueCombo)
            {
                //Debug.Log("Attac Continues");
                animator.SetInteger("Attack", animator.GetInteger("Attack") + 1);
                canContinueCombo = false;
                return;
            }
        }

        IEnumerator LookingForEnemy()
        {
            int counter = 0;
            while (true)
            {
                if (animator.GetInteger("Attack") == 0)
                {
                    Debug.Log("Stopped Looking for Enemy");
                    yield break;
                }
                //Debug.Log("Looking For Enemy");
                else if (counter == 1)
                {
                    counter = 0;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1.1f, whatIsAttackable);
                    Debug.DrawRay(transform.position, transform.right * 1.1f, Color.cyan);
                    if (hit)
                    {
                        Debug.Log("Player hit Enemy");
                        GetComponent<PlayerMovement>().SetVelocityTo0();
                        hit.transform.GetComponent<EnemyHealth>().DamageTaken(transform, forwardAttackDamage);
                        animator.SetInteger("Attack", 35);
                        yield break;
                    }
                }
                counter++;
                yield return new WaitForFixedUpdate();
            }
        }

        public void SecondaryWeaponAttack(int attack)
        {
            animator.SetInteger("Attack", attack);
        }

        public void Parry()
        {
           // Debug.Log("Start Parry");
            animator.SetInteger("Attack", 90);
        }

        void SpawnSubThrowAttack()
        {
            Vector2 spawnPosition = (Vector2)transform.position + new Vector2(weaponOriginPosition.x * transform.right.x, weaponOriginPosition.y);
            GameObject subWeaponClone = Instantiate(subWeapon, spawnPosition, Quaternion.identity, null);
            subWeaponClone.GetComponent<Projectile>().IslookingRight(transform.right);
        }
        public void MainAttackSendDamage()
        {
            SendDamage(comboAttackDamage);
        }
        void HikingSpikeAttack()
        {
            SendDamage(hikingSpikeDamage);
        }

        private void SendDamage(int damage)
        {
            Collider2D[] colls = Physics2D.OverlapCircleAll(attackTransform.position, radiusOfAttack, whatIsAttackable);
            if (colls.Length == 0) return;
            foreach (Collider2D coll in colls)
            {
                if (!coll.GetComponent<EnemyHealth>())
                {
                    coll.SendMessage("Hit");
                    return;
                }
                coll.GetComponent<EnemyHealth>().DamageTaken(transform, damage);
            }
        }     




        //Anim Events
        void SpawnBomb()
        {
            GameObject bombClone = Instantiate(bomb, attackTransform.position, Quaternion.identity);
            bombClone.GetComponent<Rigidbody2D>().velocity = new Vector2(thrownSpeed*transform.right.x, 0);
        }

        void GravityAdjust()
        {

            rb.gravityScale = 0;
            rb.sharedMaterial = GetComponent<PlayerMovement>().FullFriction();
            rb.velocity = new Vector2(0, 0);
        }

        void AddAttackImpulse(float impulse)
        {
            GetComponent<PlayerMovement>().AddImpulse(impulse);
        }

        void IsComboWindowActive(int state)
        {
            if (state == 1)
            {
                //Debug.Log("Combo Window Active");
                canContinueCombo = true;
            }
            else canContinueCombo = false;      
        }

        IEnumerator SplashAttackActions()
        {
            //Debug.Log("Anim Even Done  " + isGrounded);
            gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
            while (!GetComponent<PlayerMovement>().isGrounded)
            {
                rb.velocity = splashAttackSpeed;
                yield return new WaitForFixedUpdate();
            }
            rb.velocity = new Vector2(0, 0);
            StartCoroutine(SplashAttackDamage());
            gameObject.layer = LayerMask.NameToLayer("Player");
            yield return SplashAttackEffects();
            
        }
        IEnumerator SplashAttackDamage()
        {

            Collider2D[] colls = Physics2D.OverlapBoxAll(transform.position, splashAttackAreaSize, 0, whatIsAttackable);
            foreach (Collider2D coll in colls)
            {
                if (coll == null) break;
                EnemyHealth target = coll.GetComponent<EnemyHealth>();
                if (target == null) continue;

                coll.GetComponent<EnemyHealth>().DamageTaken(transform, splashDamage);
            }
            yield return null;
        }

        IEnumerator SplashAttackEffects()
        {
            Debug.Log("Splash Effects On");
            audioSource.PlayOneShot(splashAttackSound, 1);
            //yield return new WaitForSeconds(0.05f);
            CinemachineVirtualCamera cam = followCamera.GetComponent<CinemachineVirtualCamera>();
            CinemachineBasicMultiChannelPerlin noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            noise.m_AmplitudeGain = 1;
            yield return new WaitForSeconds(0.25f);
            float amplitude = noise.m_AmplitudeGain;
            while (noise.m_AmplitudeGain > 0)
            {
                amplitude -= Time.deltaTime * 20;
                if (amplitude <= 0) amplitude = 0;
                noise.m_AmplitudeGain = amplitude;

                yield return new WaitForEndOfFrame();
            }
        }

        void ComboAttackSound(int sound)
        {
            audioSource.PlayOneShot(attackSounds[sound-1]);
        }
        void EvadeActionEffect(int sound)
        {
            audioSource.PlayOneShot(evadeSounds[sound - 1]);

        }
        void ParrySound()
        {
            audioSource.PlayOneShot(parrySound);

        }
        void ForwardAttackSound()
        {
            audioSource.PlayOneShot(forwardAttackSound);

        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(attackTransform.position, radiusOfAttack);
        }
    }
}

