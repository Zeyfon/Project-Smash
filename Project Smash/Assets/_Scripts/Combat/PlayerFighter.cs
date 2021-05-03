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
        [SerializeField]  GameObject finisherPS = null;

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
            targetTransform.GetComponent<EnemyHealth>().SetStateToFinisher();
            gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
            RaycastHit2D hit = PositionPlayerInFronOfEnemy();
            OnFinisherCamera(true);
            //targetTransform = hit.transform;
            StartCoroutine(StartPlayerAndTargetFinisherAnimations(hit.transform));

            //isFinishinAnEnemy = false;
            yield return null;
        }

        RaycastHit2D PositionPlayerInFronOfEnemy()
        {
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
            return hit;
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
        void FinisherAttack()
        {
            if (targetTransform == null) return;
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 1), transform.right, 2, whatIsEnemy);
            if (hit)
            {
                hit.collider.GetComponent<EnemyHealth>().TakeFinisherAttackDamage(transform.position, (int)baseStats.GetStat(StatsList.Attack) * finisherAttackFactor);
                Instantiate(finisherPS, attackTransform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Enemy not spotted for finisher attack");
            }
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
        void LightAttackDamage()
        {
            //print("NormalAttack");
            Vector2 attackArea = new Vector2(1.9f, 1.6f);
            Attack(attackTransform, attackArea, fists);
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
        void SubweaponAttack()
        {
            //print("ToolAttack");
            Vector2 attackArea = new Vector2(2.5f, 1.75f);
            Attack(attackTransform, attackArea, mace);
        }

        //AnimEvent
        void SbuweaponAttackSound()
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
                //print(currentWeapon.name);
                target.TakeDamage(transform, currentWeapon, AttackType.NotUnblockable, baseStats.GetStat(StatsList.Attack));
                //print("Sendingdamage from player to  " + target);

            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireCube(attackTransform.position, comboAttackArea);

        }
    }
}

