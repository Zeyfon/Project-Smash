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


        [Header("SlopeControl")]
        public float slopeCheckDistance = 0.5f;
        public float maxSlopeAngle = 45;
        [SerializeField] PhysicsMaterial2D fullFriction = null;
        [SerializeField] PhysicsMaterial2D lowFriction = null;
        public LayerMask whatIsSlope;
        [SerializeField] Transform groundCheck = null;
        public float groundCheckRadius = 0.5f;
        [SerializeField] LayerMask whatIsGround;

        [SerializeField] bool canDebug = false;

        Animator animator;
        Rigidbody2D rb;
        SkeletonMecanim mecanim;
        Bone bone;
        SlopeControl slopes;
        EnemyHealth health;
        
        Vector2 slopeNormalPerp;
        float slopeDownAngleOld;
        float slopeDownAngle;
        float slopeSideAngle;
        float currentYAngle = 0;
        bool isOnSlope = false;
        bool canWalkOnSlope = true;
        bool isPlayerReachable = true;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            mecanim = GetComponent<SkeletonMecanim>();
            health = GetComponent<EnemyHealth>();
        }

        void Start()
        {
            bone = GetComponent<SkeletonRenderer>().skeleton.FindBone(boneName);
        }

        void FixedUpdate()
        {
            float xVelocity = (transform.right.x * rb.velocity.x) / speed;
            animator.SetFloat("xVelocity", xVelocity);
        }

        public void MoveTo(Vector3 destination, float speedFactor)
        {
            CheckFlip(destination);
            SlopeCheck();

            if (!IsGrounded())
            {
                Vector2 velocity = -slopeNormalPerp * speed * transform.right.x * speedFactor;
                velocity = new Vector2(velocity.x, 0);
            }
            else
            {
                rb.velocity = -slopeNormalPerp * speed * transform.right.x * speedFactor;
            }
        }


        public void MoveAwayFromTarget(Vector3 target, float speedFactor)
        {
            print("Moving Away from Target");
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
        void SlopeCheck()
        {
            Vector2 checkPos = transform.position;
            SlopeCheckVertical(checkPos);
            SlopeCheckHorizontal(checkPos);
        }
        void SlopeCheckHorizontal(Vector2 checkPos)
        {
            RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsSlope);
            RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsSlope);
            Debug.DrawRay(checkPos, transform.right * slopeCheckDistance, Color.blue);
            Debug.DrawRay(checkPos, -transform.right * slopeCheckDistance, Color.blue);
            if (slopeHitFront)
            {
                //print("Slope In Front");
                isOnSlope = true;
                slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
                //print(slopeSideAngle);
            }
            else if (slopeHitBack)
            {
                //print("SlopeInBack");
                isOnSlope = true;
                //slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
                slopeSideAngle = 0.0f;
            }
            else
            {
                isOnSlope = false;
                slopeSideAngle = 0.0f;
            }
        }
        void SlopeCheckVertical(Vector2 checkPos)
        {
            RaycastHit2D hit = Physics2D.Raycast(checkPos + new Vector2(0, 0.3f), Vector2.down, slopeCheckDistance, whatIsSlope);
            if (canDebug)
            {
                if (hit) Debug.Log(hit);
                else
                {
                    Debug.LogWarning(hit + "did not found anithing");
                }

            }
            if (hit)
            {
                slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
                //print(slopeNormalPerp);
                slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeDownAngle != slopeDownAngleOld)
                {
                    isOnSlope = true;
                    slopeDownAngleOld = slopeDownAngle;
                }
                Debug.DrawRay(hit.point, hit.normal, Color.green);
                Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
                //Debug.Break();
            }
            //print(canWalkOnSlope + "  " + isOnSlope);
            //print(slopeDownAngle+ "  " + slopeSideAngle);
            if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
            {
                //print(slopeDownAngle + "  " + slopeSideAngle);
                canWalkOnSlope = false;
            }
            else
            {
                canWalkOnSlope = true;
            }

            if (!canWalkOnSlope && isOnSlope)
            {
                isPlayerReachable = false;
                //Debug.Log("Player cannot be reached");
            }
            else
            {
                isPlayerReachable = true;
            }
            //print(canWalkOnSlope +" " + isOnSlope);

            //if (isOnSlope && !isPlayerNear && canWalkOnSlope)
            //{
            //    rb.sharedMaterial = fullFriction;
            //}
            //else
            //{
            //    rb.sharedMaterial = lowFriction;
            //}
        }

        public void CheckFlip(Vector3 playerPosition)
        {
            Vector2 toTarget = (playerPosition - transform.position).normalized;
            //print(toTarget + "  " + transform.right + "  " + Vector2.Dot(toTarget, transform.right));
            if (Vector2.Dot(toTarget, transform.right) > 0)
            {
                //Do Nothing
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

        public void Cancel()
        {
            //print("Cancelling Movement");
            rb.velocity = new Vector2(0, 0);
        }
    }

}
