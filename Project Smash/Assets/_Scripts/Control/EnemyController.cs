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
        EnemyFighter attack;
        EnemyHealth health;
        Vector3 originPosition;
        ActionScheduler actionScheduler;

        float timeSinceLastSawPlayer = Mathf.Infinity;
        float timeSinceAggrevated = Mathf.Infinity;
        // Start is called before the first frame update
        void Awake()
        {
            originPosition = transform.position;
            movement = GetComponent<EnemyMovement>();
            attack = GetComponent<EnemyFighter>();
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
            //if (health.IsStaggered() || health.IsStunned()|| health.IsBlocking() || health.IsBeingFinished()) return;
            //if (attack.IsAttacking()) return;
            if (IsAggrevated())
            {
                //print("AttackBehavior");
                FighterBehavior();
            }
            else if (timeSinceLastSawPlayer <= suspicionTime)
            {
                //print("Suspicion Behavior");
                SuspicionBehavior();
            }
            else
            {
                //print("Patrol Behavior");
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


        //Used by several external elements to aggrevate this enemy
        //Enemy Vision Script will use Aggrevate method to inform this enemy
        //that the enemy has been spotted
        public void Aggrevate()
        {
            print("Enemy saw");
            timeSinceAggrevated = 0;
        }

        void FighterBehavior()
        {
            timeSinceLastSawPlayer = 0;
            AggrevateNearbyEnemies();
            //This only pass the transform to the Enemy Attack script
            //so that script "has seen" he player and will go after him
            //attack.StartFightBehavior(player.transform);
        }

        void SuspicionBehavior()
        {
            actionScheduler.CancelCurrentAction();
            //attack.TargetLost();
        }

        void PatrolBehavior()
        {
            //movement.StartMoveAction(originPosition, MovementTypes.patrol);
        }

        bool IsAggrevated()
        {
            if (timeSinceAggrevated <= aggroCooldownTime)
            {
                return true;
            }

            bool inAttackRange = Vector3.Distance(transform.position, player.transform.position) < chaseRange;
            return inAttackRange /*&& movement.GetIsAtOrigin()*/;
        }

        void AggrevateNearbyEnemies()
        {
            //print("Looking to aggrevate nearby comrades");
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + new Vector3(0, 1, 0), transform.right, 2, whatIsEnemy);
            if (hits.Length == 0) return;
            foreach(RaycastHit2D hit in hits)
            {
                EnemyController controller = hit.collider.GetComponent<EnemyController>();
                if (controller == null) continue;
                //print("Aggrevating this comrade " + hit.collider.gameObject.name);
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
