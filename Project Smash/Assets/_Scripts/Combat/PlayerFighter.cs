using PSmash.Attributes;
using System;
using System.Collections;
using UnityEngine;
using PSmash.Combat.Weapons;
using PSmash.Inventories;
using PSmash.Stats;
using PSmash.Movement;

namespace PSmash.Combat
{
    public class PlayerFighter : MonoBehaviour
    {

        [Header("General Info")]
        [SerializeField] LayerMask whatIsDamagable;
        [SerializeField] LayerMask whatIsEnemy;

        [Header("Combo Attack")]
        [SerializeField] Transform attackTransform = null;
        [SerializeField] Vector2 comboAttackArea = new Vector2(1.5f, 1.5f);
        [SerializeField] AudioClip attackSound = null;
        [SerializeField] Weapon fists = null;
        [SerializeField] float attackImpulse = 10f; 

        [Header("Tool Attack")]
        [SerializeField] Weapon mace = null;
        [SerializeField] AudioClip toolAttackSound = null;

        [Header("Finishing Move")]
        [SerializeField] int finisherAttackFactor = 10;
        [SerializeField] AudioClip finisherSound = null;
        [SerializeField] Transform steamTransform = null;
        [SerializeField]  GameObject finisherEffects = null;

        [Header("Guard")]
        [SerializeField] AudioClip guardFootstepSound = null;

        public event Action AirSmashAttackEffect;
        public static event Action OnCameraShake;

        public delegate void FinisherCamera(bool enableFinisherCamera);
        public static event FinisherCamera OnFinisherCamera;


        BaseStats baseStats;
        Animator animator;
        AudioSource audioSource;
        Transform targetTransform;

        //bool isFinishinAnEnemy = false;

        void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        #region Finisher

        public bool IsEnemyStunned()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 1), transform.right, 2, whatIsEnemy);
            if (!hit) return false;
            targetTransform = hit.transform;
            return hit.transform.GetComponent<EnemyHealth>().IsStunned();
        }

        public void FinisherMove()
        {
            StartCoroutine(DoFinisherMove());
        }

        IEnumerator DoFinisherMove()
        {
            SetEnemyStateToFinisher();
            gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
            print("Player is doing Finisher Move");
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
            OnFinisherCamera(true);
            //targetTransform = hit.transform;
            StartCoroutine(StartPlayerAndTargetFinisherAnimations(hit.transform));

            //isFinishinAnEnemy = false;
            yield return null;
        }

        private void SetEnemyStateToFinisher()
        {
            targetTransform.GetComponent<EnemyHealth>().SetStateToFinisher();
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
            targetTransform.GetComponent<EnemyHealth>().DeliverFinishingBlow(transform.position, (int)baseStats.GetStat(StatsList.Attack) * finisherAttackFactor);
            //Camera Shake
            print("Camera Effect");
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
        void LightAttackDamage(int comboAttackNumber)
        {
            //print("NormalAttack");
            Attack(attackTransform, comboAttackArea, fists);
        }

        /// <summary>
        /// AnimEvent for Attacks
        /// </summary>
        /// <returns></returns>
        void AttackImpulse()
        {
            //print("AttackImpulse");
            GetComponent<PlayerMovement>().MovementImpulse(attackImpulse);
        }

        void LightAttackSound()
        {
            audioSource.pitch = UnityEngine.Random.Range(0.75f, 1.1f);
            audioSource.PlayOneShot(attackSound);
        }

        #endregion

        #region ToolAttack

        //Anim Event
        void ToolAttack()
        {
            //print("ToolAttack");
            Attack(attackTransform, comboAttackArea, mace);
        }

        //AnimEvent
        void ToolAttackSound()
        {
            audioSource.pitch = UnityEngine.Random.Range(0.75f, 1.1f);
            audioSource.PlayOneShot(toolAttackSound);
        }

        #endregion

        void Attack(Transform attackOriginPosition, Vector2 attackArea, Weapon currentWeapon)
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
                if (target == null || coll.GetComponent<Projectile>())
                    continue;
                target.TakeDamage(transform, currentWeapon, baseStats.GetStat(StatsList.Attack));
                //print("Sendingdamage from player to  " + target);

            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(attackTransform.position, comboAttackArea);

        }
    }
}

