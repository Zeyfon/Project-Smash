using UnityEngine;
using PSmash.Combat;
using PSmash.Attributes;
using PSmash.Movement;
using System;

namespace PSmash.Control
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] float chaseRange = 5f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] bool lookRight = true;

        bool rageState = false;

        public bool testMode = false;

        EnemyMovement movement;
        EnemyAttack attack;
        EnemyHealth health;
        Vector3 spawnPosition;
        EnemyVision vision;
        ActionScheduler actionScheduler;

        float timeSinceLastSawPlayer = 0;
        // Start is called before the first frame update
        void Awake()
        {
            spawnPosition = transform.position;
            movement = GetComponent<EnemyMovement>();
            attack = GetComponent<EnemyAttack>();
            health = GetComponent<EnemyHealth>();
            vision = GetComponentInChildren<EnemyVision>();
            actionScheduler = GetComponent<ActionScheduler>();
        }

        private void Start()
        {
            if (!lookRight) movement.Flip();
        }

        void Update()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (health.IsDead()) return;
            if (testMode) return;
            if (vision.Target == null) return;
            if (health.IsStaggered() || health.IsStunned()|| health.IsBlocking() || health.IsBeingFinished()) return;
            if (attack.IsAttacking()) return;
            if (IsTargetInRange())
            {
                //print("AttackBehavior");
                AttackBehavior();
            }
            else if (timeSinceLastSawPlayer <= suspicionTime)
            {
                //print("Suspicion Behavior");
                SuspicionBehavior();
            }
            else
            {
                print("Patrol Behavior");
                PatrolBehavior();
            }
        }

        void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0;
            attack.StartAttackBehavior(vision.Target);
        }

        void SuspicionBehavior()
        {
            actionScheduler.CancelCurrentAction();
        }

        void PatrolBehavior()
        {
            movement.StartMoveAction(spawnPosition, MovementTypes.patrol);
            if (Mathf.Abs(transform.position.x - spawnPosition.x) < 0.5f)
            {
                movement.CheckFlip(vision.Target.position);
                if (rageState) return;
                vision.Target = null;
                attack.targetHealth = null;
            }
        }

        bool IsTargetInRange()
        {
            if (rageState) return true;
            if (Mathf.Abs(transform.position.y - vision.Target.position.y) > 3) return false;
            else return Vector3.Distance(transform.position, vision.Target.position) < chaseRange;
        }

        public void SetAutomaticAttack(Transform target)
        {
            vision.Target = target;
            rageState = true;
        }
    }

}
