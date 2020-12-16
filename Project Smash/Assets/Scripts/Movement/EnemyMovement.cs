using PSmash.Attributes;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace PSmash.Movement
{
    public class EnemyMovement : MonoBehaviour, IAction
    {
        [SpineBone(dataField: "skeletonRenderer")]
        [SerializeField] public string boneName;

        [Header("Speed Values")]
        [SerializeField] float baseSpeed = 5;
        [SerializeField] float chaseFactor = 1;
        [SerializeField] float patrolingFactor = 0.5f;


        [Header("General Info")]
        [SerializeField] PhysicsMaterial2D fullFriction = null;
        [SerializeField] PhysicsMaterial2D lowFriction = null;
        [SerializeField] Transform groundCheck = null;
        [SerializeField] LayerMask whatIsGround;
        [SerializeField] LayerMask whatIsObstacle;
        [SerializeField] LayerMask whatIsPlayer;
        [SerializeField] float distanceCheckForObstacles = 1;

        Animator animator;
        Rigidbody2D rb;
        ActionScheduler actionScheduler;
        float currentYAngle = 0;
        bool isAtOrigin = false;
        float groundCheckRadius = 0.5f;


        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            actionScheduler = GetComponent<ActionScheduler>();
        }

        void FixedUpdate()
        {
            SetXVelocityInAnimator();
        }

        private void SetXVelocityInAnimator()
        {
            float xVelocity = (transform.right.x * rb.velocity.x) / baseSpeed;
            animator.SetFloat("xVelocity", xVelocity);
        }

        public void StartMoveAction(Vector3 destination, MovementTypes myMovement, Vector3 playerPosition)
        {
            actionScheduler.StartAction(this);
            if (Mathf.Abs(Vector3.Distance(destination, transform.position)) < 0.4f)
            {
                isAtOrigin = false;
                StopMovement();
                return;
            }
            isAtOrigin = true;
            MoveTo(destination, myMovement);
        }

        public bool GetIsAtOrigin()
        {
            return isAtOrigin;
        }


        //MoveTo and MoveAwayFrom must be combined in the future
        public void MoveTo(Vector3 targetPosition, MovementTypes myMovement)
        {
            if (IsPlayerAbove()) MoveAwayFrom(targetPosition, 0.9f);
            //print("Moving towards Target");
            if (!CanMoveTowardsTarget() )
            {
                //print("Cant reach Target");
                rb.velocity = new Vector2(0, rb.velocity.y);
                return;
            }
            //print("Can reach Target");
            if (IsGrounded())
            {
                float speed = GetSpeed(myMovement);
                rb.velocity = transform.right*speed;
            }
        }
        bool IsPlayerAbove()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 2.3f, whatIsPlayer);
            if (hit)
            {
                Debug.LogWarning("Player is above");
                return true;
            }
            else return false;
        }
        public void MoveAwayFrom(Vector3 targetPosition, float speedFactor)
        {
            FlipCheck(targetPosition);
            float targerRelativePosition = targetPosition.x - transform.position.x;
            if (targerRelativePosition > 0)
            {
                rb.MovePosition(new Vector2(1 * speedFactor, 0) + (Vector2)transform.position);
            }
            else rb.MovePosition(new Vector2(-1 * speedFactor, 0) + (Vector2)transform.position);
        }
        private float GetSpeed(MovementTypes myMovement)
        {
            float speedFactor;
            switch (myMovement)
            {
                case MovementTypes.chase:
                    speedFactor = chaseFactor;
                    break;
                case MovementTypes.patrol:
                    speedFactor = patrolingFactor;
                    break;
                default:
                    speedFactor = chaseFactor;
                    break;
            }
            return speedFactor * baseSpeed;
        }

        bool CanMoveTowardsTarget()
        {
            //if (CanIReachTheTarget(targetPosition)) return false;
            Debug.DrawRay(transform.position + transform.right + new Vector3(0, 1, 0), Vector2.down, Color.blue);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right + new Vector3(0, 1, 0), Vector2.down, 1.5f, whatIsGround);
            if (!hit)
            {
                print("There is no ground in front");
                return false; 
            }

            float angle = Vector2.Angle(Vector2.up, hit.normal);
            if (Mathf.Approximately(angle, 0))
            {
                print("There is plain groun in front");
                return true;
            }
            else
            {
                print("There is slope in front");
                return false;
            }

        }

        public bool IsPathClearToReachTarget(Vector3 targetPosition)
        {
            FlipCheck(targetPosition);
            //print("Looking for enemy in front");
            //Debug.DrawRay(transform.position + new Vector3(0, 1, 0), transform.right, Color.cyan);

            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 1), transform.right, distanceCheckForObstacles, whatIsObstacle);
            if (hit && hit.collider.gameObject != gameObject)
            {
                print("There is an obstacle between the target and me");
                return false;
            }
            else
            {
                return true;
            }
        }

        bool IsGrounded()
        {
            bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
            return isGrounded;
        }

        public void Cancel()
        {
            //print("Cancelling Movement");
            rb.velocity = new Vector2(0, 0);
        }

        public void FlipCheck(Vector3 targetPosition)
        {
            Vector2 toTarget = (targetPosition - transform.position).normalized;
            if (Vector2.Dot(toTarget, transform.right) > 0)
            {
                return;
            }
            else
            {
                Flip();
            }
        }

        public void Flip()
        {
            currentYAngle += 180;
            if (currentYAngle == 360) currentYAngle = 0;
            Quaternion currentRotation = new Quaternion(0, 0, 0, 0);
            Vector3 rotation = new Vector3(0, currentYAngle, 0);
            currentRotation.eulerAngles = rotation;
            transform.rotation = currentRotation;
        }

        public void ImpulseFromAttack(Transform attacker, float impulse)
        {

            if (attacker.position.x > transform.position.x)
            {
                rb.velocity = new Vector2(-impulse, 0);
                print("Impulse from attack " + impulse);
            }
            else
            {
                print("Impulse from attack " + impulse);
                rb.velocity = new Vector2(impulse, 0);

            }
        }

        public void Impulse(Vector2 impulse)
        {
            Vector2 speed = impulse * transform.right;
            rb.velocity = speed; 
        }
        public void StopMovement()
        {
            rb.velocity = new Vector2(0, 0);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

}
