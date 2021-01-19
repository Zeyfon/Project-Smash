using PSmash.Attributes;
using UnityEngine;

namespace PSmash.Movement
{
    public class EnemyMovement : MonoBehaviour, IAction
    {

        [Header("Speed Values")]
        [SerializeField] float baseSpeed = 5;
        [SerializeField] float chaseFactor = 1;
        [SerializeField] float patrolingFactor = 0.5f;

        [Header("Slope Control")]
        [SerializeField] float slopeCheckDistance = 0.5f;
        [SerializeField] float maxSlopeAngle = 40f;
        [SerializeField] PhysicsMaterial2D noFriction = null;
        [SerializeField] PhysicsMaterial2D lowFriction = null;
        [SerializeField] PhysicsMaterial2D fullFriction = null;


        [Header("General Info")]
        [SerializeField] Transform groundCheck = null;
        [SerializeField] LayerMask whatIsGround;
        [SerializeField] LayerMask whatIsObstacle;
        [SerializeField] LayerMask whatIsPlayer;
        [SerializeField] float distanceCheckForObstacles = 1;
        [SerializeField] float specialAttackRange = 4;
        [SerializeField] float groundCheckRadius = 0.5f;

        [Header("TestMode")]
        [SerializeField] bool isMovementTesting = false;
        
        
        Transform targetTest;
        Animator animator;
        Rigidbody2D rb;
        Vector2 slopeNormalPerp;
        bool canMidRangeBlockAttack = false;
        bool isGrounded;
        bool isOnSlope = false;
        bool canWalkOnSlope;
        float slopeDownAngle;
        float slopeDownAngleOld;
        float slopeSideAngle;
        float currentYAngle = 0;


        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
        }

        void FixedUpdate()
        {
            GroundCheck();
            SetXVelocityInAnimator();
            if(isMovementTesting)
                TestAutomaticMovement();
        }
        void TestAutomaticMovement()
        { 
            if(targetTest == null)
                targetTest = GameObject.FindGameObjectWithTag("Player").transform;
            MoveTo(targetTest.position, 1, true, null);

        }
        private void GroundCheck()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
            if (isGrounded)
            {
                //print("IsGrounded");
            }
            else
            {
                print("NotGrounded");
            }

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
            SlopeCheck(transform.right.x);
            //print("Moving towards " + targetPosition);
            CheckWhereToFace(targetPosition, isMovingTowardsTarget);
            if (IsPlayerAbove())
            {
                print("Player is above");
                MoveAwayFrom(targetPosition, 0.9f);
            }
            //print("Moving towards Target");
            else if (CanMoveToTheFront() && isGrounded)
            {
                //print("SlopeNormalPerp.x = " +slopeNormalPerp.x + "  SlopeNormalPerp.y  = " + slopeNormalPerp.y);
                float speed = baseSpeed * speedFactor;
                float xVelocity = -1  *speed * slopeNormalPerp.x;
                float yVelocity = -1 * speed * slopeNormalPerp.y;
                rb.velocity = new Vector2(xVelocity, yVelocity);
                print("XVelocity = " + xVelocity + "  yVelocity  = " + yVelocity + "  transform.right.x  =" + transform.right.x );
                //.velocity = transform.right * speed;
            }
            else if (!isGrounded)
            {
                print("Falling till reaching floor");
            }
            else if(CanMidRangeBlockAttack() && !CanMoveToTheFront() && isGrounded && IsTargetInSpecialAttackRange(targetPosition))
            {
                print("Sending to State " + pm.FsmName + " SPECIAL ATTACK Event ");
                //Debug.Break();
                if (pm == null)
                    return;
                pm.SendEvent("SPECIALATTACK");
            }
            else if (Mathf.Approximately(slopeNormalPerp.x,1))
            {
                rb.velocity = new Vector2(0,rb.velocity.y);
            }
            else
            {
                Debug.LogWarning("Does not what to do in MoveTo Method ");
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }

        #region SlopeControl
        public void SlopeCheck(float xInput)
        {
            Vector2 checkPos = transform.position;
            SlopeCheckVertical(checkPos, xInput);
            SlopeCheckHorizontal(checkPos);
        }
        void SlopeCheckHorizontal(Vector2 checkPos)
        {
            RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsGround);
            RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsGround);
            Debug.DrawRay(checkPos, transform.right * slopeCheckDistance, Color.blue);
            Debug.DrawRay(checkPos, -transform.right * slopeCheckDistance, Color.blue);
            print(slopeHitFront.normal);
            if (slopeHitFront && !slopeHitFront.collider.CompareTag("LadderTop") && Mathf.Abs(slopeHitFront.normal.x) < 0.9f)
            {
                isOnSlope = true;
                slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
            }
            else if (slopeHitBack && !slopeHitBack.collider.CompareTag("LadderTop") && Mathf.Abs(slopeHitBack.normal.x) < 0.9f)
            {
                isOnSlope = true;
                slopeSideAngle = 0.0f;
            }
            else
            {
                isOnSlope = false;
                slopeSideAngle = 0.0f;
            }
        }
        void SlopeCheckVertical(Vector2 checkPos, float xInput)
        {
            RaycastHit2D hit = Physics2D.Raycast(checkPos + new Vector2(0,.2f), Vector2.down, slopeCheckDistance, whatIsGround);
            if (hit)
            {
                slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized * transform.right.x;
                slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeDownAngle != slopeDownAngleOld)
                {
                    isOnSlope = true;
                    slopeDownAngleOld = slopeDownAngle;
                }
                Debug.DrawRay(hit.point, hit.normal, Color.green);
                Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            }
            else
            {
                print("No ground found in vertical Slope Check");
            }
            if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
            {
                canWalkOnSlope = false;
            }
            else
            {
                canWalkOnSlope = true;
            }

            if (isOnSlope && xInput == 0.0f && canWalkOnSlope)
            {
                rb.sharedMaterial = fullFriction;
            }
            else
            {
                rb.sharedMaterial = lowFriction;
            }
        }
        #endregion

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
            Debug.DrawRay(transform.position + transform.right + new Vector3(0, 0.5f, 0), Vector2.down, Color.blue);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + transform.right + new Vector3(0, .5f, 0), Vector2.down, 2f, whatIsGround);
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
                if (canWalkOnSlope)
                    return true;
                else
                    return false;
            }
        }
        private bool IsObstacleInFront()
        {
            //FlipCheck(targetPosition);
            //print("Looking for enemy in front");
            //Debug.DrawRay(transform.position + new Vector3(0, 1, 0), transform.right, Color.cyan);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 1), transform.right, distanceCheckForObstacles, whatIsObstacle);
            if (hit && (hit.collider.gameObject != gameObject && !hit.collider.CompareTag("Ladder")))
            {
                print(hit.collider.gameObject.tag);
                print("There is an obstacle between the target and me");
                return true;
            }
            else
            {
                print("There is no obstacle between the target and me");
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
