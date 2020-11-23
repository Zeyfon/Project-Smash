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

        Animator animator;
        Rigidbody2D rb;
        SkeletonMecanim mecanim;
        Bone bone;
        ActionScheduler actionScheduler;
        float currentYAngle = 0;
        bool isPlayerReachable = true;
        float groundCheckRadius = 0.5f;


        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            mecanim = GetComponent<SkeletonMecanim>();
            bone = GetComponent<SkeletonRenderer>().skeleton.FindBone(boneName);
            actionScheduler = GetComponent<ActionScheduler>();
        }

        void FixedUpdate()
        {

            //This only is for the animator to keep track of xVelocity
            float xVelocity = (transform.right.x * rb.velocity.x) / baseSpeed;
            animator.SetFloat("xVelocity", xVelocity);
        }

        public void StartMoveAction(Vector3 destination, MovementTypes myMovement)
        {
            actionScheduler.StartAction(this);
            MoveTo(destination, myMovement);
        }


        //MoveTo and MoveAwayFrom must be combined in the future
        public void MoveTo(Vector3 target, MovementTypes myMovement)
        {
            CheckFlip(target);
            //print("Moving to Target");
            if (!CanMoveTowardsTarget())
            {
                //Will not move
                //print("Cant reach Target");
                return;
            }
            //print("Can reach Target");

            if (IsGrounded())
            {
                float speed = GetSpeed(myMovement);
                rb.velocity = transform.right*speed;
            }
        }

        public void MoveAwayFrom(Vector3 target, float speedFactor)
        {
            print("Moving Away from Target");
            CheckFlip(target);

            float targerRelativePosition = target.x - transform.position.x;
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
            Debug.DrawRay(transform.position + transform.right + new Vector3(0,1,0), Vector2.down,Color.blue);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right + new Vector3(0, 1, 0), Vector2.down, 2, whatIsGround);
            if (!hit) return false;
            float angle = Vector2.Angle(Vector2.up, hit.normal);
            if (Mathf.Approximately(angle, 0)) return true;
            return false;
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

        public void CheckFlip(Vector3 playerPosition)
        {
            Vector2 toTarget = (playerPosition - transform.position).normalized;
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

        //Anim Event
        void SetNewPosition()
        {
            Vector3 newPosition = mecanim.transform.TransformPoint(new Vector3(bone.WorldX, bone.WorldY, 0f));
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 0.5f), transform.right, Vector3.Distance(newPosition, transform.position), whatIsGround);
            if (hit)
            {
                return;
            }
            newPosition = new Vector3(newPosition.x, newPosition.y, 0);
            transform.position = newPosition;
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

}
