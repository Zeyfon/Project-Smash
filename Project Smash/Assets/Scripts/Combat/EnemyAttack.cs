using PSmash.Core;
using PSmash.Resources;
using System.Collections;
using UnityEngine;
using PSmash.Movement;
using Spine.Unity;

namespace PSmash.Combat
{
    public class EnemyAttack : MonoBehaviour, IAction
    {
        [SerializeField] Material normalMaterial;
        [SerializeField] Material unblockableMaterial;
        [SerializeField] float fadeIntTime = 0.5f;

        [Range(0,100)]
        [SerializeField] float unblockableAttackProbability = 0.2f;
        [SerializeField] LayerMask whatIsAttackable;
        [SerializeField] float attackRange = 2;
        [SerializeField] Transform attackTransform = null;
        [SerializeField] float attackRadius = 1;
        [SerializeField] int damage = 10;
        [SerializeField] PhysicsMaterial2D fullFriction = null;
        [SerializeField] PhysicsMaterial2D lowFriction = null;
        [SerializeField] float getParriedDistance = 2;
        [SerializeField] LayerMask whatIsPlayerGuard;
        [SerializeField] LayerMask whatIsPlayer;
        [SerializeField] AudioClip parriedSound = null;
        [SerializeField] AudioClip hitGuardSound = null;
        [SerializeField] Vector2[] attackArea;
        [SerializeField] AudioClip[] attackSounds = null;
        Animator animator;
        EnemyMovement movement;
        public PlayerHealth target = null;
        EnemyHealth health;
        Rigidbody2D rb;
        Coroutine attackCoroutine;
        AudioSource audioSource;
        bool isNormalAttacking = false;
        bool isUnblockableAttacking = false;
        bool isParried = false;
        int id = 0;

        // Start is called before the first frame update
        void Start()
        {
            animator = GetComponent<Animator>();
            movement = GetComponent<EnemyMovement>();
            health = GetComponent<EnemyHealth>();
            rb = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Update()
        {
            if (health.IsDead()) return;
            if (target == null) return;
            if (health.IsStaggered() ||health.IsStunned() || health.IsBlocking() || health.IsBeingFinished()) return;
            if (isNormalAttacking) return;
            if (!IsTargetInRange())
            {
                if (PlayerIsAbove())
                {
                    movement.MoveAwayFromTarget(target.transform.position,0.2f);
                    return;
                }
                movement.MoveTo(target.transform.position, 1f);
            }
            else
            {
                AttackBehaviour();
            }
        }
        public void StartAttackBehavior(Transform targetTransform)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            if (target == null) target = targetTransform.GetComponent<PlayerHealth>();
        }

        void AttackBehaviour()
        {
            if (attackCoroutine != null) return;
            float random = Random.Range(0, 100);
            if (random < unblockableAttackProbability)
            {
                attackCoroutine = StartCoroutine(UnblockableAttack());
            }
            else
            {
                attackCoroutine = StartCoroutine(ComboAttack());
            }
        }

        IEnumerator ComboAttack()
        {
            isNormalAttacking = true;
            rb.sharedMaterial = fullFriction;
            movement.CheckFlip(target.transform.position);
            animator.SetInteger("attack", 1);
            while (animator.GetInteger("attack") != 100)
            {
                yield return new WaitForEndOfFrame();
            }
            rb.sharedMaterial = lowFriction;
            yield return new WaitForSeconds(1);
            //print("Combo Attack Finished");
            isNormalAttacking = false;
            attackCoroutine = null;
        }

        IEnumerator UnblockableAttack()
        {
            print("Unblockable Attacking");
            isUnblockableAttacking = true;
            GetComponent<SkeletonRenderer>().CustomMaterialOverride.Add(normalMaterial, unblockableMaterial);
            yield return null;
            //print("Material Overriden");
            foreach (Material material in GetComponent<MeshRenderer>().materials)
            {
                //print(material.name);
                StartCoroutine(FadeIn(material));

            }
            //StartCoroutine(FadeIn(unblockableMaterial));
            rb.sharedMaterial = fullFriction;
            movement.CheckFlip(target.transform.position);
            animator.SetInteger("attack", 10);
            while (animator.GetInteger("attack") != 100)
            {
                yield return new WaitForEndOfFrame();
            }
            animator.SetInteger("attack", 0);
            rb.sharedMaterial = lowFriction;
            GetComponent<SkeletonRenderer>().CustomMaterialOverride.Add(unblockableMaterial, normalMaterial);
            GetComponent<SkeletonRenderer>().CustomMaterialOverride.Remove(unblockableMaterial);
            GetComponent<SkeletonRenderer>().CustomMaterialOverride.Remove(normalMaterial);
            attackCoroutine = null;
            isUnblockableAttacking = false;
            print(gameObject.name + "  Unblockable Attack Finished");
        }

        IEnumerator FadeIn(Material currentMaterial)
        {
            Color tintColor = currentMaterial.GetColor("_Tint");
            float alpha = 0;
            while (alpha != 1)
            {
                alpha += Time.deltaTime/fadeIntTime;
                if (alpha >= 1) alpha = 1;

                currentMaterial.SetColor("_Tint", new Color(tintColor.r, tintColor.g, tintColor.b, alpha));
                //print(currentMaterial.GetColor("_Tint"));
                yield return null;
            }
        }

        //The hit directly checks if the player is parrying, guarding, attacking, etc.
        //It puts the hits in the order they are perceived being the parry the first
        //since the trigger collider is a little in front of the other in the player
        void Hit(int id)
        {
            audioSource.PlayOneShot(attackSounds[id-1]);
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + new Vector3(0.3f, 1), transform.right, 2, whatIsAttackable);
            if (hits.Length == 0) return;
            foreach (RaycastHit2D hit in hits)
            {
                if (!isUnblockableAttacking & hit.collider.CompareTag("Parry"))
                {
                    hit.collider.transform.parent.GetComponent<PlayerFighterV2>().StartParry();
                    isParried = true;
                    audioSource.PlayOneShot(parriedSound);
                    GetComponent<IDamagable>().TakeDamage(hit.collider.transform.parent, 25);
                    break;
                }
                if (!isUnblockableAttacking & hit.collider.CompareTag("Guard"))
                {
                    audioSource.PlayOneShot(hitGuardSound);
                    return;
                }
                IDamagable target = hit.collider.GetComponent<IDamagable>();
                if (target == null) continue;
                target.TakeDamage(transform, damage);
            }
        }

        bool PlayerIsAbove()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 2.3f, whatIsPlayer);
            if (hit)
            {
                return true;
            }
            else return false;
        }

        public void Cancel()
        {
            //print(this + "  Cancelled");
            target = null;
            if (animator.GetInteger("attack") > 0 && attackCoroutine != null)
            {
                animator.SetInteger("attack", 100);
            }
        }

        public bool IsAttacking()
        {
            if (isNormalAttacking || isUnblockableAttacking) return true;
            else return false;
        }

        public bool IsParried
        {
            get
            {
                return isParried;
            }
            set
            {
                isParried = value;
            }
        }
        bool IsTargetInRange()
        {
            bool isInRange = Mathf.Abs(target.transform.position.x - transform.position.x) < attackRange;
            return isInRange;
        }

        public bool GetIsUnblockableAttacking()
        {
            return isUnblockableAttacking;
        }
        public bool GetIsNormalAttacking()
        {
            return isNormalAttacking;
        }

        private void OnDrawGizmos()
        {
            if (id == 0 && !isNormalAttacking) return;
            Gizmos.DrawWireCube(attackTransform.position, attackArea[id]);
        }
    }
}


