using UnityEngine;
using PSmash.Combat;
using PSmash.Resources;
using PSmash.Core;
using PSmash.Movement;

namespace PSmash.Control
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] float attackRange = 1.5f;
        [SerializeField] float chaseRange = 5f;
        [SerializeField] float movementRange = 20f;
        [SerializeField] float suspicionTime = 3f;
        [SerializeField] bool lookRight = true;
        public bool testMode = false;
        EnemyMovement movement;
        Transform playerTransform;
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
            if (!isPlayerSpotted) return;
            if (health.IsInterrupted()) return;
            if (health.IsStunned()) return;
            if (attack.IsAttacking()) return;
            if (testMode) return;
            if (InAttackRange() && InMovementArea() && !isMovingToSpawnPosition)
            {
                //print("AttackBehavior");
                AttackBehavior();
                return;
            }
            else if (timeSinceLastSawPlayer <= suspicionTime)
            {
                //print("Suspicion Behavior");
                SuspicionBehavior();
                return;
            }
            else if(!InOriginPosition())
            {
                //print("Patrol Behavior");
                PatrolBehavior();
            }
        }

        void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0;
            if(attack.target == null) attack.StartAttackBehavior(playerTransform);
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
            }
            else
            {
                movement.MoveTo(spawnPosition, 0.3f);
            }
        }

        bool InAttackRange()
        {
            bool inAttackRange = Vector3.Distance(transform.position, playerTransform.position) < chaseRange;
            return inAttackRange;
        }

        bool InMovementArea()
        {
            bool inMovementArea = Vector3.Distance(transform.position, spawnPosition) < movementRange;
            return inMovementArea;
        }

        bool InOriginPosition()
        {
            bool inOriginPosition = Vector3.Distance(transform.position, spawnPosition) < 0.5f;
            if (inOriginPosition) ReactivatePlayerSpotterTrigger();
            return inOriginPosition;
        }
        void ReactivatePlayerSpotterTrigger()
        {
            GetComponentInChildren<BoxCollider2D>().enabled = true;
            isPlayerSpotted = false;
            isMovingToSpawnPosition = false;
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
            this.playerTransform = playerTransform;
            this.isPlayerSpotted = isPlayerSpotted;
        }
    }

}
