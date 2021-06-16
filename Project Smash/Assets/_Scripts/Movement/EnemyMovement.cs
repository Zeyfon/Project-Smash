using System.Collections;
using UnityEngine;

namespace PSmash.Movement
{
    public class EnemyMovement : CharacterMovement
    {
        [System.Serializable]
        struct AttackMovements
        {
            public float impulseTime;
            public float speedFactor;
        }
        //CONFIG
        [Header("Speed Values")]
        [SerializeField] float speedFactor = 1;
        

        [Header("Slope Control")]
        [SerializeField] float maxSlopeAngle = 40f;

        [Header("General Info")]
        [SerializeField] Transform groundCheck = null;

        [SerializeField] float distanceCheckForObstacles = 1;
        [SerializeField] float groundCheckRadius = 0.5f;

        [Header("Layers Masks")]
        [SerializeField] LayerMask whatIsObstacle;
        [SerializeField] LayerMask whatIsPlayer;

        [Header("Attack/Damage Movements")]
        [Tooltip("The times each attack impulse will be active")]
        [SerializeField] AttackMovements[] attackMovements;


        //STATE
        Animator animator;
        Vector2 impulseDirection;
        bool isGrounded;
        float currentYAngle = 0;
        float speedMovementModifier = 1;
        Coroutine coroutine;
        bool isMovementOverriden = false;
        bool isAttackingWithImpulse = false;
        float impulseTime = 0;
        float timer = Mathf.Infinity;
        float speedModifier = 1;
        Transform player;
        Vector2 direction;
        bool hasFinished = false;

        //INITIALIZE
        // Start is called before the first frame update
        void Awake()
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponent<Animator>();
            if (animator == null)
                animator = GetComponentInChildren<Animator>();
        }

        void FixedUpdate()
        {
            GroundCheck();
            SetXVelocityInAnimator();
            if (timer < impulseTime)
            {
                CombatImpulse();
                print(timer);
                hasFinished = true;
            }
            else if (hasFinished)
            {
                rb.velocity = new Vector2(0, 0);
                hasFinished = false;
            }

            timer += Time.deltaTime;
        }

        void SetAttackImpulseDirection()
        {
            direction = GetMovementDirection();
        }

        /// <summary>
        /// Anim Event to start the movement caused by attack. Completely optional
        /// </summary>
        /// <param name="index"></param>
        void StartAttackImpulse(int index)
        {
            impulseTime = attackMovements[index].impulseTime;
            speedFactor = attackMovements[index].speedFactor;
            timer = 0;
        }

        void CombatImpulse()
        {
            
            print("impulsing" + direction + "  " + speedFactor + "  " + speedModifier);
            Movement(direction, true, speedFactor, speedModifier);
        }

        Vector2 GetMovementDirection()
        {
            float xPosition = player.position.x - transform.position.x;
            if (xPosition > 0)
            {
                print("Moving to the right");
                return new Vector2(1, 0);

            }
            else
            {
                print("Moving to the left");
                return new Vector2(-1, 0);
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
            if (Mathf.Abs(targetPosition.x - transform.position.x) < 0.2f)
                return;
            CheckWhereToFace(targetPosition, isMovingTowardsTarget);
            if (IsTargetAbove())
            {
                //print("Player is above");
                MoveAwayFrom(targetPosition, 0.9f);
            }
            else if (!IsAtOrigin(targetPosition)&& CanMove())
            {
                 //print("Is In Slope");
                if (!slope.CanWalkOnThisGround(transform.position, transform.right, slopeCheckDistance, maxSlopeAngle, whatIsGround))
                {
                rb.sharedMaterial = noFriction;
                }
                else
                {
                    rb.sharedMaterial = lowFriction;
                    if (!isMovingTowardsTarget)
                    {
                        targetPosition = InvertTargetPosition(targetPosition);
                    }
                    Vector2 movementDirectionNormalized = (targetPosition - transform.position).normalized;
                    Movement(movementDirectionNormalized, isGrounded, speedFactor, speedMovementModifier);
                }
            }
            else
            {
                rb.sharedMaterial = lowFriction;
            }
        }

        Vector3 InvertTargetPosition(Vector3 positionToInvert)
        {
            float distance = positionToInvert.x - transform.position.x;
            if (distance > 0)
                return new Vector3(transform.position.x + distance, positionToInvert.y, positionToInvert.z);
            else
                return new Vector3(transform.position.x - distance, positionToInvert.y, positionToInvert.z);

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
            isMovementOverriden = true;
            
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
        public void CheckWhereToFace(Vector3 targetPosition, bool isMovingTowardsTarget)
        {
            Vector3 forward = transform.TransformDirection(Vector3.right);
            Vector3 toOther = (targetPosition - transform.position).normalized;
            //print(forward + "  " + toOther + "  "  +  Vector3.Dot(forward,toOther));
            bool isPlayerInFront = Vector3.Dot(forward, toOther) >  0 ;

            //To rotate and moves towards the target
            if(!isPlayerInFront && isMovingTowardsTarget)
            {

                Flip();
            }

            //To rotate and moves away from target without facing it. RUNNING AWAY
            else if(isPlayerInFront && !isMovingTowardsTarget)
            {
                Flip();
            }

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

        bool IsTargetAbove()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 2.3f, whatIsPlayer);
            if (hit)
            {
                return true;
            }
            else return false;
        }
        IEnumerator SpecialAttackImpulse_CR(float speedFactor)
        {
            SlopeControl slope = new SlopeControl();
            Vector2 direction;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                direction = slope.GetMovementDirectionWithSlopecontrol(transform.position, transform.right, slopeCheckDistance, whatIsGround);
                Movement(direction, isGrounded, speedFactor, speedMovementModifier);
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
            isMovementOverriden = true;
            yield return null;
            float tempDrag = rb.drag;
            PhysicsMaterial2D tempPhysicsMaterial = rb.sharedMaterial;
            Vector2 attackDirection = (transform.position - attacker.position).normalized;
            float timer = 0;
            float speedFactor = attackForce / baseSpeed;
            float timerLimit = Mathf.Log10(attackForce)/ ((attackForce/2.4f)+1);

            rb.sharedMaterial = lowFriction;
            rb.drag = 2;

            while (timer < timerLimit)
            {
                timer += Time.fixedDeltaTime;

                impulseDirection = slope.GetMovementDirectionWithSlopecontrol(transform.position, attackDirection, slopeCheckDistance, whatIsGround);
                Movement(impulseDirection, isGrounded, speedFactor, speedMovementModifier);
                yield return new WaitForFixedUpdate();
            }
            rb.drag = tempDrag;
            rb.sharedMaterial = tempPhysicsMaterial;
            isMovementOverriden = false;
        }



        bool CanMove()
        {
            if (IsGroundInFront() && !IsObstacleInFront() && !isMovementOverriden) return true;
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
