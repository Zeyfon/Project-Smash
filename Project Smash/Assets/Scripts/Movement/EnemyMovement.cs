using PSmash.Control;
using System.Collections;
using UnityEngine;
using Spine.Unity;
using Spine;
using PSmash.Resources;

namespace PSmash.Movement
{
    public class EnemyMovement : MonoBehaviour, IAction
    {
        [SpineBone(dataField: "skeletonRenderer")]
        [SerializeField] public string boneName;
        [SerializeField] float speed = 5;


        [Header("General Info")]
        [SerializeField] PhysicsMaterial2D fullFriction = null;
        [SerializeField] PhysicsMaterial2D lowFriction = null;
        [SerializeField] Transform groundCheck = null;
        [SerializeField] LayerMask whatIsGround;

        Animator animator;
        Rigidbody2D rb;
        SkeletonMecanim mecanim;
        Bone bone;        
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
        }

        void FixedUpdate()
        {
            float xVelocity = (transform.right.x * rb.velocity.x) / speed;
            animator.SetFloat("xVelocity", xVelocity);
        }

        public void MoveTo(Vector3 target, float speedFactor)
        {
            CheckFlip(target);
            if (IsGrounded())
            {
                rb.velocity = speed * transform.right * speedFactor;
            }
        }


        public void MoveAwayFromTarget(Vector3 target, float speedFactor)
        {
            print("Moving Away from Target");
            CheckFlip(target);

            float playerRelativePosition = target.x - transform.position.x;
            if (playerRelativePosition > 0)
            {
                rb.MovePosition(new Vector2(1 * speedFactor, 0) + (Vector2)transform.position);
            }
            else rb.MovePosition(new Vector2(-1 * speedFactor, 0) + (Vector2)transform.position);
        }

        public void StartMoveAction(Vector3 destination, float speedFactor)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, speedFactor);
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

        public bool IsPlayerReachable()
        {
            return isPlayerReachable;
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
