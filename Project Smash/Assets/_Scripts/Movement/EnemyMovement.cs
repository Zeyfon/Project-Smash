using PSmash.Attributes;
using System;
using System.Collections;
using UnityEngine;

namespace PSmash.Movement
{
    public class EnemyMovement : MonoBehaviour
    {
        //CONFIG
        [Header("Speed Values")]
        [SerializeField] float baseSpeed = 5;
        //[SerializeField] float chaseFactor = 1;
        //[SerializeField] float patrolingFactor = 0.5f;

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
        

        //STATE
        Animator animator;
        Rigidbody2D rb;
        bool isGrounded;
        float currentYAngle = 0;
        float speedMovementModifier = 1;
        Coroutine coroutine;

        int test = 0;
        bool isMovementInterruption = false;

        //INITIALIZE
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
            if (isMovementInterruption)
            {
                //print("velocity " + rb.velocity);
            }
        }

        //////////////////////////////////////////////////////////////////////////////PUBLIC/////////////////////////////////////////////////////////////////////////// 
        public void SetFullFrictionMaterial()
        {
            rb.sharedMaterial = fullFriction;
        }

        //MoveTo and MoveAwayFrom must be combined in the future
        public void MoveTo(Vector3 targetPosition, float speedFactor, bool isMovingTowardsTarget, PlayMakerFSM pm)
        {
            CheckWhereToFace(targetPosition, isMovingTowardsTarget);
            if (IsTargetAbove())
            {
                //print("Player is above");
                MoveAwayFrom(targetPosition, 0.9f);
            }
            else if (!isGrounded)
            {
               // rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else if (isGrounded && !IsAtOrigin(targetPosition))
            {
                if (!isMovementInterruption)
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
                                rb.sharedMaterial = lowFriction;
                                Vector2 direction = slope.GetMovementDirectionWithSlopecontrol(transform.position, transform.right, slopeCheckDistance, whatIsGround);
                                Move(direction, speedFactor, speedMovementModifier);
                            }
                            else
                            {
                                //print("Not Moving");
                                rb.sharedMaterial = fullFriction;
                                Move(new Vector2(0, 0), 0, 0);
                            }
                        }
                        else
                        {
                            //print("IsMoving");
                            rb.sharedMaterial = lowFriction;
                            Vector2 direction = slope.GetMovementDirectionWithSlopecontrol(transform.position, transform.right, slopeCheckDistance, whatIsGround);
                            Move(direction, speedFactor, speedMovementModifier);
                        }
                    }
                    else
                    {
                        //print("Not Moving");
                        rb.sharedMaterial = fullFriction;
                        Move(new Vector2(0, 0), 0, 0);
                    }
                }
                else
                {
                    print("Being moved by attack force");
                    //Let the Physics Engine takes control by itself
                }

            }
            else
            {
                //Debug.LogWarning(gameObject.name + "  does not what to do in MoveTo Method ");
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        /// <summary>
        /// Disables movement control by other objects.
        /// Apply the attack force to the entity.
        /// Waits till it passes the force to restore movement control.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="attackForce"></param>
        public void ApplyAttackImpactReceived(Transform attacker, float attackForce)
        {
            StartCoroutine(ApplyAttackImpactReceived_CR(attacker,attackForce));
        }

        /// <summary>
        /// Will update the movemement speed by its factor from 0 to 1;
        /// Example: The speed of the smasher after losing its armor will be greater than before.
        /// </summary>
        /// <param name="speedMovementModifier"></param>
        public void SetSpeedMovementModifierValue(float speedMovementModifier)
        {
            this.speedMovementModifier = speedMovementModifier;
            //print("Movement Speed Increased");
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

        //Used by FSMs
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

        //////////////////////////////////////////////////////////////////////////////PRIVATE//////////////////////////////////////////////////////////////////////////
        bool IsAtOrigin(Vector3 originPosition)
        {
            if (Mathf.Abs(originPosition.x - transform.position.x) < 0.5f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        IEnumerator SpecialAttackImpulse_CR(float speedFactor)
        {
            SlopeControl slope = new SlopeControl();
            Vector2 slopeNormalPerp;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                slopeNormalPerp = slope.GetMovementDirectionWithSlopecontrol(transform.position, transform.right, slopeCheckDistance, whatIsGround);
                Move(slopeNormalPerp, speedFactor,1);
            }
        }

        void GroundCheck()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
            //print("Is Grounded  " + isGrounded);
        }


        void SetXVelocityInAnimator()
        {
            if (animator == null) return;
            float xVelocity = (transform.right.x * rb.velocity.x) / baseSpeed;
            animator.SetFloat("xVelocity", xVelocity);
        }

        IEnumerator ApplyAttackImpactReceived_CR(Transform attacker, float attackForce)
        {
            //print("Force applyied to " + gameObject.name);
            isMovementInterruption = true;
            yield return null;
            float tempDrag = rb.drag;
            PhysicsMaterial2D tempPhysicsMaterial = rb.sharedMaterial;
            rb.sharedMaterial = lowFriction;
            rb.drag = 2;
            Vector2 attackDirection = (transform.position - attacker.position).normalized;
            float timer = 0;
            SlopeControl slope = new SlopeControl();
            float speedFactor = attackForce / baseSpeed;
            rb.velocity = new Vector2(0, 0);
            float timerLimit = Mathf.Log10(attackForce)/ ((attackForce/2.4f)+1) ;
            //print("Timerlimit  " + timerLimit);
            while (timer < timerLimit)
            {
                //print("Force applied is " + speedFactor);
                timer += Time.fixedDeltaTime;
                //print(timer);

                Vector2 direction = slope.GetMovementDirectionWithSlopecontrol(transform.position, attackDirection, slopeCheckDistance, whatIsGround);
                //print(direction + "  " + speedFactor);
                Move(direction, speedFactor, 1);
                yield return new WaitForFixedUpdate();
            }
            rb.drag = tempDrag;
            rb.sharedMaterial = tempPhysicsMaterial;
            isMovementInterruption = false;
        }



        bool CanMove()
        {
            if (IsGroundInFront() && !IsObstacleInFront()) return true;
            else return false;
        }

        void Move(Vector2 direction, float speedFactor, float speedModifier)
        {
            float speed = baseSpeed * speedFactor * speedModifier;

            float xVelocity = speed * direction.x;
            float yVelocity = speed * direction.y;

            rb.velocity = new Vector2(xVelocity, yVelocity);
        }
        void StayStill()
        {

            //rb.velocity = new Vector2(0, rb.velocity.y);
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

        bool IsGroundInFront()
        {
            Debug.DrawRay(transform.position + transform.right + new Vector3(0, 0.5f, 0), Vector2.down, Color.blue);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right + new Vector3(0, .5f, 0), Vector2.down, 2f, whatIsGround);
            if (!hit)
                return false;
            else
                return true;
        }
        bool IsObstacleInFront()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 1), transform.right, distanceCheckForObstacles, whatIsObstacle);
            if (hit && (hit.collider.gameObject != gameObject && !hit.collider.CompareTag("Ladder")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

}
