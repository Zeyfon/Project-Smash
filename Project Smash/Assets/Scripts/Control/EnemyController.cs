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
        [SerializeField] float aggroCooldownTime = 3f;
        [SerializeField] float aggroRadius = 3;
        [SerializeField] LayerMask whatIsEnemy;

        [SerializeField] bool lookRight = true;
        public bool testMode = false;

        bool rageState = false;

        Transform player;
        EnemyMovement movement;
        EnemyAttack attack;
        EnemyHealth health;
        Vector3 spawnPosition;
        ActionScheduler actionScheduler;

        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        // Start is called before the first frame update
        void Awake()
        {
            spawnPosition = transform.position;
            movement = GetComponent<EnemyMovement>();
            attack = GetComponent<EnemyAttack>();
            health = GetComponent<EnemyHealth>();
            actionScheduler = GetComponent<ActionScheduler>();
            player = GameObject.FindGameObjectWithTag("Player").transform;
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
            if (health.IsStaggered() || health.IsStunned()|| health.IsBlocking() || health.IsBeingFinished()) return;
            if (attack.IsAttacking()) return;
            if (IsAggrevated())
            {
                print("AttackBehavior");
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
            UpdateTimers();
        }

        private void UpdateTimers()
        {
            timeSinceLastSawPlayer += Time.deltaTime;
            if (rageState) return;
            timeSinceAggrevated += Time.deltaTime;
        }

        public void Aggrevate()
        {
            timeSinceAggrevated = 0;
        }

        void AttackBehavior()
        {
            timeSinceLastSawPlayer = 0;
            AggrevateNearbyEnemies();
            attack.StartAttackBehavior(player.transform);
        }

        void SuspicionBehavior()
        {
            actionScheduler.CancelCurrentAction();
        }

        void PatrolBehavior()
        {
            movement.StartMoveAction(spawnPosition, MovementTypes.patrol,player.transform.position);
        }

        bool IsAggrevated()
        {
            if (timeSinceAggrevated <= aggroCooldownTime) return true;
            bool inAttackRange = Vector3.Distance(transform.position, player.transform.position) < chaseRange;
            return inAttackRange && movement.GetIsReturningToOrigin();
        }

        void AggrevateNearbyEnemies()
        {
            //RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, aggroRadius, transform.right, 0, whatIsEnemy);
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + new Vector3(0, 1, 0), transform.right, 2, whatIsEnemy);
            //print(hits.Length);
            if (hits.Length == 0) return;
            foreach(RaycastHit2D hit in hits)
            {
                //print("Looking to aggrevate");
                EnemyController controller = hit.collider.GetComponent<EnemyController>();
                if (controller == null) continue;
                //print(gameObject.name + " is aggrevating " + controller.gameObject.name);
                controller.Aggrevate();
            }
        }

        public void EnableRageState()
        {
            rageState = true;
            Aggrevate();
        }
    }

}
