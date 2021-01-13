using PSmash.Attributes;
using Spine;
using Spine.Unity;
using System;
using UnityEngine;

namespace PSmash.Movement
{
    public class EnemyMovement : MonoBehaviour, IAction
    {

        [Header("Speed Values")]
        [SerializeField] float baseSpeed = 5;
        [SerializeField] float chaseFactor = 1;
        [SerializeField] float patrolingFactor = 0.5f;


        [Header("General Info")]
        [SerializeField] Transform groundCheck = null;
        [SerializeField] LayerMask whatIsGround;
        [SerializeField] LayerMask whatIsObstacle;
        [SerializeField] LayerMask whatIsPlayer;
        [SerializeField] float distanceCheckForObstacles = 1;
        [SerializeField] float specialAttackRange = 4;

        Animator animator;
        Rigidbody2D rb;
        bool canMidRangeBlockAttack = false;
        float currentYAngle = 0;
        float groundCheckRadius = 0.5f;


        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        void FixedUpdate()
        {
            SetXVelocityInAnimator();
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
            if (Mathf.Abs(targetPosition.x - transform.position.x) < 0.5f)
                return;
            //print("Moving to target");
            CheckWhereToFace(targetPosition, isMovingTowardsTarget);
            if (IsPlayerAbove())
            { 
                MoveAwayFrom(targetPosition, 0.9f);
            }
            //print("Moving towards Target");
            else if (CanMoveToTheFront() && IsGrounded())
            {
                float speed = baseSpeed * speedFactor;
                rb.velocity = transform.right * speed;
            }
            else if (!IsGrounded())
            {
                print("Falling till reaching floor");
                //rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            }
            else if(CanMidRangeBlockAttack() && !CanMoveToTheFront() && IsGrounded() && IsTargetInSpecialAttackRange(targetPosition))
            {
                print("Sending to State " + pm.FsmName + " SPECIAL ATTACK Event ");
                //Debug.Break();
                pm.SendEvent("SPECIALATTACK");
            }
            else
            {
                Debug.LogWarning("Does not what to do in MoveTo Method ");
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        private bool IsTargetInSpecialAttackRange(Vector3 targetPosition)
        {
            print(specialAttackRange +"  "+ Vector3.Distance(transform.position, targetPosition));
            return specialAttackRange > Vector3.Distance(transform.position, targetPosition);
        }

        bool IsPlayerAbove()
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
            print(gameObject.name + " is moving away ");
            CheckWhereToFace(targetPosition, false);
            float targerRelativePosition = targetPosition.x - transform.position.x;
            if (targerRelativePosition > 0)
            {
                rb.MovePosition(new Vector2(1 * speedFactor, 0) + (Vector2)transform.position);
            }
            else rb.MovePosition(new Vector2(-1 * speedFactor, 0) + (Vector2)transform.position);
        }

        bool CanMoveToTheFront()
        {
            if (IsGroundInFrontWalkable() && !IsObstacleInFront()) return true;
            else return false;
        }

        private bool IsGroundInFrontWalkable()
        {
            Debug.DrawRay(transform.position + transform.right + new Vector3(0, 1, 0), Vector2.down, Color.blue);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right + new Vector3(0, 1, 0), Vector2.down, 1.5f, whatIsGround);
            if (!hit)
            {
                //print("There is no ground in front");
                return false;
            }

            float angle = Vector2.Angle(Vector2.up, hit.normal);
            if (Mathf.Approximately(angle, 0))
            {
                //print("There is plain groun in front");
                return true;
            }
            else
            {
                //print("There is slope in front");
                return false;
            }
        }
        private bool IsObstacleInFront()
        {
            //FlipCheck(targetPosition);
            //print("Looking for enemy in front");
            //Debug.DrawRay(transform.position + new Vector3(0, 1, 0), transform.right, Color.cyan);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 1), transform.right, distanceCheckForObstacles, whatIsObstacle);
            if (hit && hit.collider.gameObject != gameObject)
            {
                //print("There is an obstacle between the target and me");
                return true;
            }
            else
            {
                return false;
            }
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

            //Vector2 toTarget;
            //if(isFacingTarget)
            //    toTarget = (targetPosition - transform.position).normalized;
            //else
            //    toTarget = (transform.position- targetPosition).normalized;
            //if (Vector2.Dot(toTarget, transform.right) > 0)
            //{
            //    return;
            //}
            //else
            //{
            //    Flip();
            //}
        }

        public void SetCanMidRangeBlockAttack(bool state)
        {
            canMidRangeBlockAttack = state;
        }
        private bool CanMidRangeBlockAttack()
        {
            return canMidRangeBlockAttack;
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
