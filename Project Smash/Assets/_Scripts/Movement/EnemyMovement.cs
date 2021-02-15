using PSmash.Attributes;
using System;
using System.Collections;
using UnityEngine;

namespace PSmash.Movement
{
    public class EnemyMovement : MonoBehaviour
    {

        [Header("Speed Values")]
        [SerializeField] float baseSpeed = 5;
        [SerializeField] float chaseFactor = 1;
        [SerializeField] float patrolingFactor = 0.5f;

        [Header("Slope Control")]
        [SerializeField] float slopeCheckDistance = 0.5f;
        [SerializeField] float maxSlopeAngle = 40f;

        [Header("General Info")]
        [SerializeField] Transform groundCheck = null;

        [SerializeField] float distanceCheckForObstacles = 1;
        [SerializeField] float groundCheckRadius = 0.5f;

        [Header("Physics Materials")]
        [SerializeField] PhysicsMaterial2D lowFriction = null;
        [SerializeField] PhysicsMaterial2D fullFriction = null;

        [Header("Layers Masks")]
        [SerializeField] LayerMask whatIsGround;
        [SerializeField] LayerMask whatIsObstacle;
        [SerializeField] LayerMask whatIsPlayer;
        
        Animator animator;
        Rigidbody2D rb;
        bool isGrounded;
        float currentYAngle = 0;
        Coroutine coroutine;

        int test = 0;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }

        void FixedUpdate()
        {
            GroundCheck();
            SetXVelocityInAnimator();
        }
        private void GroundCheck()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
            //print("Is Grounded  " + isGrounded);
        }


        private void SetXVelocityInAnimator()
        {
            if (animator == null) return;
            float xVelocity = (transform.right.x * rb.velocity.x) / baseSpeed;
            animator.SetFloat("xVelocity", xVelocity);
        }

        //MoveTo and MoveAwayFrom must be combined in the future
        public void MoveTo(Vector3 targetPosition, float speedFactor, bool isMovingTowardsTarget, PlayMakerFSM pm)
        {
            //print("MOVING TO");
            if(isGrounded)
                rb.sharedMaterial = lowFriction;

            if (Mathf.Abs(targetPosition.x - transform.position.x) < 0.5f)
                return;
            CheckWhereToFace(targetPosition, isMovingTowardsTarget);
            if (IsTargetAbove())
            {
                //print("Player is above");
                MoveAwayFrom(targetPosition, 0.9f);
            }
            else if (!isGrounded)
            {
                //print("Falling till reaching floor");
            }
            else if (isGrounded)
            {
                if (CanMove())
                {
                    SlopeControl slope = new SlopeControl();
                    //print("Is Grounded and can move");
                    if (slope.IsOnSlope(transform.position, transform.right, slopeCheckDistance, whatIsGround))
                    {
                        //print("Is In Slope");
                        if (slope.CanWalkOnSlope(transform.position, transform.right, slopeCheckDistance, maxSlopeAngle, whatIsGround))
                        {
                            //print("Is Moving");
                            Move(slope.GetSlopeNormalPerp(transform.position, transform.right, slopeCheckDistance, maxSlopeAngle, whatIsGround), speedFactor);
                        }
                        else
                        {
                            //print("Is Not Moving");
                            DoNotMove();
                        }
                    }
                    else
                    {
                        //print("IsMoving");
                        Move(slope.GetSlopeNormalPerp(transform.position, transform.right, slopeCheckDistance, maxSlopeAngle, whatIsGround), speedFactor);
                    }
                }
                else
                {
                    //print("Is Not Moving");
                    DoNotMove();
                }
            }
            else
            {
                Debug.LogWarning(gameObject.name + "  does not what to do in MoveTo Method ");
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        bool CanMove()
        {
            if (IsGroundInFront() && !IsObstacleInFront()) return true;
            else return false;
        }

        void Move(Vector2 slopeNormalPerp, float speedFactor )
        {
            //print("Moving");
            //print("Previous velocity " + rb.velocity);
            float speed = baseSpeed * speedFactor;
            float xVelocity = -1 * speed * slopeNormalPerp.x;
            float yVelocity = -1 * speed * slopeNormalPerp.y;

            rb.velocity = new Vector2(xVelocity, yVelocity);
            //print("Post velocity " + rb.velocity);
        }
        void DoNotMove()
        {
            //print("Not Moving");
                rb.sharedMaterial = fullFriction;
            rb.velocity = new Vector2(0, rb.velocity.y);
        }

        bool IsTargetAbove()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 2.3f, whatIsPlayer);
            if (hit)
            {
                //Debug.LogWarning("Player is above");
                return true;
            }
            else return false;
        }
        public void MoveAwayFrom(Vector3 targetPosition, float speedFactor)
        {
            //print(gameObject.name + " is moving away ");
            CheckWhereToFace(targetPosition, false);
            float targerRelativePosition = targetPosition.x - transform.position.x;
            if (targerRelativePosition > 0)
            {
                rb.MovePosition(new Vector2(1 * speedFactor, 0) + (Vector2)transform.position);
            }
            else rb.MovePosition(new Vector2(-1 * speedFactor, 0) + (Vector2)transform.position);
        }

        private bool IsGroundInFront()
        {
            Debug.DrawRay(transform.position + transform.right + new Vector3(0, 0.5f, 0), Vector2.down, Color.blue);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right + new Vector3(0, .5f, 0), Vector2.down, 2f, whatIsGround);
            if (!hit)
                return false;
            else
                return true;
        }
        private bool IsObstacleInFront()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 1), transform.right, distanceCheckForObstacles, whatIsObstacle);
            if (hit && (hit.collider.gameObject != gameObject && !hit.collider.CompareTag("Ladder")))
            {
                print(hit.collider.gameObject.name);
                //print("There is an obstacle between the target and me");
                return true;
            }
            else
            {
                //print("There is no obstacle between the target and me");
                return false;
            }
        }

        public void CheckWhereToFace(Vector3 targetPosition, bool isFacingTarget)
        {
            bool playerIsAtRight = targetPosition.x - transform.position.x > 0;
            bool isLookingRight = transform.right.x > 0;

            if (playerIsAtRight && isLookingRight && isFacingTarget)
                return;
            else if (playerIsAtRight && isLookingRight && !isFacingTarget)
                Flip();
            else if (playerIsAtRight && !isLookingRight && isFacingTarget)
                Flip();
            else if (playerIsAtRight && !isLookingRight && !isFacingTarget)
                return;

            else if (!playerIsAtRight && isLookingRight && isFacingTarget)
                Flip();
            else if (!playerIsAtRight && isLookingRight && !isFacingTarget)
                return;
            else if (!playerIsAtRight && !isLookingRight && isFacingTarget)
                return;
            else if (!playerIsAtRight && !isLookingRight && !isFacingTarget)
                Flip();
        }

        public void Flip()
        {
            //print("Flipping");
            currentYAngle += 180;
            if (currentYAngle == 360) currentYAngle = 0;
            Quaternion currentRotation = new Quaternion(0, 0, 0, 0);
            Vector3 rotation = new Vector3(0, currentYAngle, 0);
            currentRotation.eulerAngles = rotation;
            transform.rotation = currentRotation;
        }

        public void SpecialAttackImpulse_Start(float speedFactor)
        {
            rb.sharedMaterial = lowFriction;
            gameObject.layer = LayerMask.NameToLayer("EnemiesGhost");
            coroutine = StartCoroutine(SpecialAttackImpulse_CR(speedFactor));
        }

        public void SpecialAttackImpulse_Stop()
        {
            rb.sharedMaterial = fullFriction;
            gameObject.layer = LayerMask.NameToLayer("Enemies");
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        public IEnumerator SpecialAttackImpulse_CR(float speedFactor)
        {
            SlopeControl slope = new SlopeControl();
            Vector2 slopeNormalPerp;
            while(true)
            {
                yield return new WaitForFixedUpdate();
                slopeNormalPerp = slope.GetSlopeNormalPerp(transform.position, transform.right, slopeCheckDistance, maxSlopeAngle, whatIsGround);
                Move(slopeNormalPerp, speedFactor);
            }
        }
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

}
