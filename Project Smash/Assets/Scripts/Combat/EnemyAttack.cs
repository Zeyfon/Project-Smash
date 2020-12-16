using PSmash.Attributes;
using PSmash.Movement;
using Spine.Unity;
using System.Collections;
using UnityEngine;

namespace PSmash.Combat
{
    public class EnemyAttack : MonoBehaviour, IAction
    {
        [SerializeField] Material normalMaterial = null;
        [SerializeField] Material unblockableMaterial = null;
        [SerializeField] float fadeIntTime = 0.5f;

        [Range(0,100)]
        [SerializeField] float unblockableAttackProbability = 0.2f;
        [SerializeField] LayerMask whatIsAttackable;
        [SerializeField] float attackRange = 2;
        [SerializeField] int damage = 10;
        [SerializeField] PhysicsMaterial2D fullFriction = null;
        [SerializeField] PhysicsMaterial2D lowFriction = null;
        [SerializeField] LayerMask whatIsPlayer;
        [SerializeField] AudioClip parriedSound = null;
        [SerializeField] AudioClip hitGuardSound = null;
        [SerializeField] Vector2[] attackArea;
        [SerializeField] AudioClip comboAttackSound = null;
        [SerializeField] AudioClip unblockableAttackSound = null;
        [SerializeField] float unblockableAttackMaxDistance = 3;
        [SerializeField] Vector2 unblockableAttackImpulse;
        public PlayerHealth targetHealth = null;

        Animator animator;
        EnemyMovement movement;
        EnemyHealth health;
        Rigidbody2D rb;
        Coroutine attackCoroutine;
        AudioSource audioSource;
        ActionScheduler actionScheduler;
        bool isNormalAttacking = false;
        bool isUnblockableAttacking = false;
        bool canDamageTarget = false;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            movement = GetComponent<EnemyMovement>();
            health = GetComponent<EnemyHealth>();
            audioSource = GetComponent<AudioSource>();
            actionScheduler = GetComponent<ActionScheduler>();
        }

        private void Update()
        {
            if (health.IsDead()) return;
            if (targetHealth == null) return;
            if (health.IsStaggered() ||health.IsStunned() || health.IsBlocking() || health.IsBeingFinished()) return;
            if (IsAttacking()) return;

            if (movement.IsPathClearToReachTarget(targetHealth.transform.position))
            {
                if (IsTargetInCloseAttackRange())
                {
                    CloseRangeAttackBehavior();
                    return;
                }
                else
                {
                    movement.MoveTo(targetHealth.transform.position, MovementTypes.chase);
                    return;
                }
            }
            else
            {
                if (IsTargetInRangeForRangedAttack())
                {
                    RangeAttackBehavior();
                    return;
                }
            }
            movement.StopMovement();
        }

         public void StartFightBehavior(Transform target)
        {
            actionScheduler.StartAction(this);
            this.targetHealth = target.GetComponent<PlayerHealth>();
        }

        public void TargetLost()
        {
            targetHealth = null;
        }

        void CloseRangeAttackBehavior()
        {
            if (attackCoroutine != null) return;
            float random = Random.Range(0, 100);
            if (random > unblockableAttackProbability)
            {
                //print("Normal Attack");
                NormalAttack();
            }
            else
            {
                //print("Unblockable Attacking");
                UnblockableAttack();
            }
        }

        private void NormalAttack()
        {
            if (attackCoroutine != null) return;
            attackCoroutine = StartCoroutine(NormalAttackCR());
        }

        private void UnblockableAttack()
        {
            if (attackCoroutine != null) return;
            attackCoroutine = StartCoroutine(UnblockableAttackCR());
        }
        private void RangeAttackBehavior()
        {
            UnblockableAttack();
        }

        private bool CanCounterAttack()
        {
            if (health.ConsecutiveBlocks == 3)
            {
                health.ConsecutiveBlocks = 0;
                return true;
            }
            return false;
        }

        IEnumerator NormalAttackCR()
        {
            isNormalAttacking = true;
            health.SetIsNormalAttacking(true);
            rb.sharedMaterial = fullFriction;
            movement.FlipCheck(targetHealth.transform.position);
            animator.SetInteger("attack", 1);
            while (animator.GetInteger("attack") != 100)
            {
                yield return new WaitForEndOfFrame();
            }
            rb.sharedMaterial = lowFriction;
            isNormalAttacking = false;
            health.SetIsNormalAttacking(false);
            yield return new WaitForSeconds(1);
            //print("Combo Attack Finished");
            attackCoroutine = null;
        }

        IEnumerator UnblockableAttackCR()
        {
            health.SetAccumulatedDamage(0);
            isUnblockableAttacking = true;
            health.SetIsUnblockableAttacking(true);
            GetComponent<SkeletonRenderer>().CustomMaterialOverride.Add(normalMaterial, unblockableMaterial);
            yield return null;
            //print("Material Overriden");
            foreach (Material material in GetComponent<MeshRenderer>().materials)
            {
                //print(material.name);
                StartCoroutine(FadeIn(material));
            }
            StartCoroutine(FadeIn(unblockableMaterial));
            //rb.sharedMaterial = fullFriction;
            float temporalDrag = rb.drag;
            rb.drag = 6;
            movement.FlipCheck(targetHealth.transform.position);
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
            health.SetIsUnblockableAttacking(false);
            rb.drag = temporalDrag;
            gameObject.layer = LayerMask.NameToLayer("Enemies");
            //print(gameObject.name + "  Unblockable Attack Finished");
        }

        bool IsTargetInCloseAttackRange()
        {
            bool isInRange = Mathf.Abs(targetHealth.transform.position.x - transform.position.x) < attackRange;
            return isInRange;
        }

        private bool IsTargetInRangeForRangedAttack()
        {
            float distance = Vector3.Distance(transform.position, targetHealth.transform.position);
            if (distance < unblockableAttackMaxDistance) return true;
            else return false;
        }

        //private void OtherActions()
        //{
        //    movement.StopMovement();
        //}

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

        //AnimEvent
        void UnblockableAttackImpulse()
        {
            //print("Impulsed");
            gameObject.layer = LayerMask.NameToLayer("EnemiesGhost");
            movement.Impulse(unblockableAttackImpulse);
        }

        void Hit()
        {
            //print("Hit");
            if(!isUnblockableAttacking) PlaySound(comboAttackSound,0.6f,1);
            else PlaySound(unblockableAttackSound, 0.4f, 0.6f);

            StartCoroutine(StartSendDamageToTarget());
        }

        IEnumerator StartSendDamageToTarget()
        {
            canDamageTarget = true;
            if (!isUnblockableAttacking) SendDamageToTarget();
            else
            {
                bool isTargetDamaged = false;
                while(canDamageTarget && !isTargetDamaged)
                {
                    isTargetDamaged = SendDamageToTarget();
                    yield return null;
                }
            }
        }

        bool SendDamageToTarget()
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + new Vector3(0.3f, 1), transform.right, 1.5f, whatIsAttackable);
            if (hits.Length == 0) return false;

            foreach (RaycastHit2D hit in hits)
            {
                IDamagable target = hit.collider.GetComponent<IDamagable>();
                if (target == null) continue;
                if (isUnblockableAttacking && hit.collider.GetComponent<PlayerGuard>()) continue;
                target.TakeDamage(transform, damage);
                //print("Sending Damage to Target");
                return true;
            }
            return false;
        }

        //AnimEvent
        void DisableCanDamageTarget()
        {
            canDamageTarget = false;
        }


        private void PlaySound(AudioClip clip, float minRange, float maxRange)
        {
            audioSource.clip = clip;
            audioSource.pitch = Random.Range(minRange, maxRange);
            audioSource.Play();
        }


        public void Cancel()
        {
            //print(this + "  Cancelled");
            //targetHealth = null;
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

        public bool GetIsUnblockableAttacking()
        {
            return isUnblockableAttacking;
        }
        public bool GetIsNormalAttacking()
        {
            return isNormalAttacking;
        }
    }
}


