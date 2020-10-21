using UnityEngine;
using PSmash.Combat;
using PSmash.Resources;
using PSmash.Movement;
using System;

namespace PSmash.Control
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] float attackRange = 1.5f;
        [SerializeField] float chaseRange = 5f;
        [SerializeField] float movementRange = 20f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] bool lookRight = true;
        [SerializeField] LayerMask whatIsGround;
        public bool testMode = false;
        EnemyMovement movement;
        Transform target;
        EnemyAttack attack;
        EnemyHealth health;
        Vector3 spawnPosition;

        float timeSinceLastSawPlayer = 0;
        bool isPlayerSpotted = false;
        bool isPlayerReachable = true;
        bool isMovingToSpawnPosition = false;
        // Start is called before the first frame update
        void Awake()
        {
            spawnPosition = transform.position;
            movement = GetComponent<EnemyMovement>();
            attack = GetComponent<EnemyAttack>();
            health = GetComponent<EnemyHealth>();
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
            if (target ==null) return;
            if (health.IsInterrupted()) return;
            if (health.IsStunned()) return;
            if (testMode) return;
            if (attack.IsAttacking()) return;
            if (InMovementArea() && InAttackRange() && isPlayerSpotted && CanMoveForward())
            {
                //print("AttackBehavior");
                AttackBehavior();
                isMovingToSpawnPosition = false;
                return;
            }
            else if (isPlayerSpotted && timeSinceLastSawPlayer <= suspicionTime)
            {
                //print("Suspicion Behavior");
                SuspicionBehavior();
                isMovingToSpawnPosition = false;
                return;
            }
            else if(!InOriginPosition())
            {
                //print("Patrol Behavior");
                PatrolBehavior();
            }
        }

        private bool CanMoveForward()
        {
            Vector2 origin = (Vector2)transform.position + new Vector2(0, 0.5f);
            Debug.DrawRay(origin, transform.right);
            bool hit = Physics2D.Raycast(origin, transform.right, 1, whatIsGround);
            if (hit)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0;
            if(attack.target == null) attack.StartAttackBehavior(target);
        }

        void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        void PatrolBehavior()
        {
            if (!isMovingToSpawnPosition)
            {
                movement.StartMoveAction(spawnPosition, 0.3f);
                isMovingToSpawnPosition = true;
                ResetIsPlayerSpotted();
            }
            else
            {
                movement.MoveTo(spawnPosition, 0.3f);
            }
        }

        private void ResetIsPlayerSpotted()
        {
            print("Reset Spotter");
            transform.GetChild(0).transform.GetComponent<Collider2D>().enabled = true;
            PlayerSpotted(target, false);
        }

        bool InAttackRange()
        {
            if (Mathf.Abs(transform.position.y - target.position.y) < 3)
            {
                bool inAttackRange = Vector3.Distance(transform.position, target.position) < chaseRange;
                return inAttackRange;
            }
            else
            {
                return false;
            }
        }

        bool InMovementArea()
        {
            bool inMovementArea = Vector3.Distance(target.position, transform.parent.position) < movementRange;
            return inMovementArea;
        }

        bool InOriginPosition()
        {
            float distanceToOrigin= Vector3.Distance(transform.position, transform.parent.position);
            bool inOriginPosition =  distanceToOrigin < 0.5f;
            if (inOriginPosition && distanceToOrigin <5) PlayerSpotted(null, false);
            return inOriginPosition;
        }

        public bool IsPlayerSpotter
        {
            get
            {
                return isPlayerSpotted;
            }
            set
            {
                print("PlayerSpotted");
                isPlayerSpotted = value;
            }
        }

        public void PlayerSpotted(Transform playerTransform, bool isPlayerSpotted)
        {
            this.target = playerTransform;
            this.isPlayerSpotted = isPlayerSpotted;
        }
        private void OnDrawGizmos()
        {
            if (transform.parent == null) return;
            Gizmos.DrawWireSphere(transform.parent.position, movementRange);
        }

        void FlipTowardsTarget()
        {
            movement.CheckFlip(target.position);
        }
    }

}
