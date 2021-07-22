using PSmash.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Inventories;
using PSmash.Stats;
using PSmash.Movement;

namespace PSmash.Combat
{
    public class PlayerFighter : MonoBehaviour
    {

        [Header("General Info")]
        [SerializeField] LayerMask whatIsDamagable;
        [SerializeField] LayerMask whatIsHooked;
        [SerializeField] LayerMask whatIsEnemy;
        [SerializeField] float attackImpulse = 12f;

        [Header("Combo Attack")]
        [SerializeField] Transform attackTransform = null;

        [Header("Tool Attack")]
        [SerializeField] AudioClip toolAttackSound = null;
        [SerializeField] AudioClip noHookSound = null;
        [SerializeField] Weapon grapingHook = null;
        [SerializeField] float grapingHookRadius = 11;
        [SerializeField] HookRope hookRope = null;

        [Header("Finishing Move")]
        [SerializeField] AudioClip finisherSound = null;
        [SerializeField] Transform steamTransform = null;
        [SerializeField]  GameObject finisherPS = null;

        [Header("Guard")]
        [SerializeField] AudioClip guardFootstepSound = null;

        public static event Action OnCameraShake;

        public delegate void FinisherCamera(bool enableFinisherCamera);
        public static event FinisherCamera OnFinisherCamera;

        BaseStats baseStats;
        Animator animator;
        AudioSource audioSource;
        Transform targetTransform;
        Transform enemyTargetTransform;
        Vector2 damageArea;
        PlayerMovement movement;

        void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            movement = GetComponent<PlayerMovement>();
        }

        #region Finisher

        public bool IsEnemyStunned()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 1), transform.right, 2, whatIsEnemy);
            if (!hit) 
                return false;
            targetTransform = hit.transform;
            return hit.transform.GetComponent<EnemyHealth>().IsStunned();
        }


        public void FinisherMove()
        {
            StartCoroutine(DoFinisherMove());
        }

        IEnumerator DoFinisherMove()
        {
            targetTransform.GetComponent<EnemyHealth>().SetStateToFinisher();
            gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
            PositionPlayerInFronOfEnemy();
            OnFinisherCamera(true);
            StartCoroutine(StartPlayerAndTargetFinisherAnimations(targetTransform));
            yield return null;
        }

        void PositionPlayerInFronOfEnemy()
        {
            if (transform.position.x - targetTransform.transform.position.x > 0)
            {
                //Player is at right side of enemy
                GetComponent<Rigidbody2D>().MovePosition(targetTransform.position + new Vector3(1, 0, 0));
            }
            else
            {
                //Player is at left side of enemy
                GetComponent<Rigidbody2D>().MovePosition(targetTransform.position + new Vector3(-1, 0, 0));
            }
        }
        IEnumerator StartPlayerAndTargetFinisherAnimations(Transform targetTransform)
        {
            animator.SetInteger("Attack", 80);
            while (animator.GetInteger("Attack") != 81)
            {
                yield return null;
            }
            targetTransform.GetComponent<EnemyHealth>().StartFinisherAnimation();
            yield return null;
        }

        public void EndingFinisherMove()
        {
            OnFinisherCamera(false);
        }

        public void GrapingHook()
        {
            if (GrapingHookTarget.TargetTransform !=null)
            {
                StartCoroutine(PulledTowardsTarget(GrapingHookTarget.TargetTransform));
                return;
            }

            RaycastHit2D hit = Physics2D.Raycast(attackTransform.position, transform.right, 10, whatIsHooked);    
            if (hit)
            {
                //print("Enemy Detected");
                enemyTargetTransform = hit.collider.transform;
                if (CanEnemyBePulled(enemyTargetTransform))
                {
                    //print("Enemy can be hooked");
                    enemyTargetTransform.GetComponent<IGrapingHook>().Hooked();
                    animator.SetInteger("Attack", 72);
                }
                else
                {
                    //print("Enemy cannot be hooked");
                    audioSource.PlayOneShot(noHookSound);
                }
            }
            else
            {
                print("Did find nothing to grab");
            }
        }

        private bool CanEnemyBePulled(Transform enemyTransform)
        {
            return !enemyTransform.GetComponent<IWeight>().IWeight();
        }

        //Anim Event
        public void PullEnemyAction()
        {
            StartCoroutine(PullingEnemyWithGrapingHook());
        }

        IEnumerator PullingEnemyWithGrapingHook()
        {
            List<Transform> points = new List<Transform>();
            points.Add(attackTransform);
            points.Add(enemyTargetTransform);
            HookRope rope = Instantiate(hookRope, attackTransform.position, Quaternion.identity, attackTransform);
            yield return rope.DoHookShot(points);
            rope.EnemyPulled(enemyTargetTransform);
            float distance = Mathf.Infinity;
            enemyTargetTransform.GetComponent<IGrapingHook>().Pulled();
            while (distance>1.25)
            {
                print("Checking Distance");
                distance = Vector3.Distance(enemyTargetTransform.position, transform.position);
                print(distance);
                yield return null;
            }
            animator.SetInteger("Attack", 75);
        }


        //DO NOT DELETE. POSSIBLE USAGE IN THE FUTURE
        IEnumerator PulledTowardsTarget(Transform targetTransform)
        {
            animator.SetInteger("Attack", 73);
            List<Transform> points = new List<Transform>();
            points.Add(attackTransform);
            points.Add(targetTransform);
            HookRope rope = Instantiate(hookRope, attackTransform.position, Quaternion.identity, attackTransform);
            yield return rope.DoHookShot(points);
            rope.PlayerPulled();

            float speed = 30;
            Vector2 direction = ((targetTransform.position + new Vector3(0, 2)) - transform.position).normalized;
            movement.GrapingHookMovement(direction, speed);
            float distance = Mathf.Infinity;
            while ( distance> 4)
            {
                //print("Distance from player to " + targetTransform.name + " is  "  + distance );
                distance = Vector3.Distance(targetTransform.position, transform.position);
                movement.GrapingHookMovement(direction, speed);
                yield return new WaitForFixedUpdate();

            }
            animator.SetInteger("Attack", 75);
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
        void FinisherAttack()
        {
            float finisherDamage = ((int)baseStats.GetStat(StatsList.Attack) + GetComponent<Equipment>().GetMainWeapon().GetDamage()) * 2;
            print(targetTransform.name);
            targetTransform.GetComponent<EnemyHealth>().TakeFinisherAttackDamage(transform.position, finisherDamage);
            Instantiate(finisherPS, attackTransform.position, Quaternion.identity);
            OnCameraShake();
        }

        //Anim Event
        void FinisherSound()
        {
            audioSource.pitch = 0.3f;
            audioSource.PlayOneShot(finisherSound);
        }

        #endregion

        #region Guard/Parry
        //AnimEvent
        void GuardFootstepSound()
        {
            audioSource.PlayOneShot(guardFootstepSound);
        }

        #endregion

        #region ComboAttack
        //Anim Event
        void AttackDamage(int index)
        {
            //print("NormalAttack");
            Weapon weapon = GetComponent<Equipment>().GetMainWeapon();
            damageArea = weapon.GetWeaponDamageArea();
            //damageArea = new Vector2(1.9f, 1.6f);
            Attack(attackTransform, weapon);
        }

        /// <summary>
        /// AnimEvent for Attacks
        /// </summary>
        /// <returns></returns>
        void AttackImpulse(int index)
        {
            GetComponent<PlayerMovement>().MovementImpulse(attackImpulse);
        }

        void AttackSound(int index)
        {
            audioSource.pitch = UnityEngine.Random.Range(0.75f, 1.1f);
            audioSource.PlayOneShot(GetComponent<Equipment>().GetMainWeapon().GetWeaponAttackAudioClip());
        }

        #endregion

        #region SubWeaponAttack

        //Anim Event
        void SubWeaponAttackDamage()
        {
            //print("SubWeapon Damage");
            Weapon weapon = GetComponent<Equipment>().GetSubWeapon();
            damageArea = weapon.GetWeaponDamageArea();
            //damageArea = new Vector2(4.5f, 1.75f);
            Attack(attackTransform, weapon);
        }

        //AnimEvent
        void SubWeaponAttackSound(int index)
        {
            //if(index == 1)
            //{
                audioSource.pitch = UnityEngine.Random.Range(0.75f, 1.5f);
                audioSource.PlayOneShot(GetComponent<Equipment>().GetSubWeapon().GetWeaponAttackAudioClip());
            //}

        }

        #endregion

        void Attack(Transform attackOriginPosition, Weapon weapon)
        {
            //print("Looking to Damage Enemy");
            Collider2D[] colls = Physics2D.OverlapBoxAll(attackOriginPosition.position, damageArea, 0, whatIsDamagable);
            if (colls.Length == 0)
            {
                //print("Nothing was damaged");
                return;
            }
            foreach (Collider2D coll in colls)
            {
                IDamagable target = coll.GetComponent<IDamagable>();
                if (target == null || coll.GetComponent<Projectile>())
                    continue;
                //print(currentWeapon.name);
                target.TakeDamage(transform, weapon, AttackType.NotUnblockable, baseStats.GetStat(StatsList.Attack), weapon.GetAttackForce());
                //print("Sendingdamage from player to  " + target);

            }
        }

        //private void OnTriggerEnter2D(Collider2D collision)
        //{
        //    if (collision.CompareTag("HookTarget"))
        //    {
        //        targetTransform = collision.transform;
        //        print("Target is Detected");
        //    }
        //}

        //private void OnTriggerExit2D(Collider2D collision)
        //{
        //    if (collision.CompareTag("HookTarget"))
        //    {
        //        targetTransform = null;
        //        print("Target is not detected");
        //    }
        //}

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(attackTransform.position, damageArea);
        }
    }
}

