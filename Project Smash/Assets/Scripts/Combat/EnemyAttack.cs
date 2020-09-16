using PSmash.Core;
using PSmash.Resources;
using System.Collections;
using UnityEngine;
using PSmash.Movement;

namespace PSmash.Combat
{
    public class EnemyAttack : MonoBehaviour, IAction
    {
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
        bool isAttacking = false;
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
            if (health.IsInterrupted()) return;
            if (health.IsStunned()) return;
            if (target == null) return;
            if (isAttacking) return;
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
            isAttacking = true;
            if (attackCoroutine != null) return;
            attackCoroutine = StartCoroutine(Attack());
        }

        IEnumerator Attack()
        {
            rb.sharedMaterial = fullFriction;
            movement.CheckFlip(target.transform.position);
            animator.SetInteger("attack", 1);
            while (animator.GetInteger("attack") != 100)
            {
                yield return new WaitForEndOfFrame();
            }
            animator.SetInteger("attack", 0);
            rb.sharedMaterial = lowFriction;
            yield return new WaitForSeconds(1);
            isAttacking = false;
            attackCoroutine = null;
        }

        void Hit(int id)
        {
            audioSource.PlayOneShot(attackSounds[id-1]);
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + new Vector3(0, 1), transform.right, 2, whatIsAttackable);
            if (hits.Length == 0) return;
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.CompareTag("Parry"))
                {
                    hit.collider.transform.parent.GetComponent<PlayerFighterV2>().StartParry();
                    health.SetIsParried(true);
                    audioSource.PlayOneShot(parriedSound);
                    GetComponent<IDamagable>().TakeDamage(hit.collider.transform.parent, 60);
                    break;
                }
                if (hit.collider.CompareTag("Guard"))
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
            return isAttacking;
        }
        bool IsTargetInRange()
        {
            bool isInRange = Mathf.Abs(target.transform.position.x - transform.position.x) < attackRange;
            return isInRange;
        }

        private void OnDrawGizmos()
        {
            if (id == 0 && !isAttacking) return;
            Gizmos.DrawWireCube(attackTransform.position, attackArea[id]);
        }
    }
}


