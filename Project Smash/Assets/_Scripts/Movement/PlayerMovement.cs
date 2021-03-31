using HutongGames.PlayMaker;
using PSmash.Attributes;
using PSmash.SceneManagement;
using Spine.Unity;
using System.Collections;
using UnityEngine;

namespace PSmash.Movement
{
    public class PlayerMovement : MonoBehaviour
    {

        #region Inspector
        //[SpineBone(dataField: "skeletonRenderer")]
        //[SerializeField] public string boneName;

        [Header("GroundCheck")]
        [SerializeField] LayerMask whatIsGround;
        [SerializeField] float groundCheckRadius = 0.1f;
        [SerializeField] Transform groundCheck = null;
        [SerializeField] AudioSource footStepAudioSource = null;
        [SerializeField] AudioSource climbingLegeAudioSource = null;
        [SerializeField] AudioClip landingSound = null;


        [Header("Ladder")]
        //[SerializeField] float maxLadderMovementSpeed = 3;
        [SerializeField] float checkLadderDistance = 1;
        [SerializeField] LayerMask whatIsLadder;
        [SerializeField] LayerMask whatIsLadderTop;
        [SerializeField] Transform ladderCheck = null;
        [SerializeField] AudioClip climbingLadderSound = null;
        [SerializeField] AudioClip climbingWallSound = null;
        

        [Header("Movement")]
        [SerializeField] float maxRunningSpeed = 5;
        //[SerializeField] float maxWallSpeed = 2;
        [SerializeField] float maxInteractingSpeed = 2;
        //[SerializeField] float maxGuardingSpeed = 2;
        //[SerializeField] float jumpVelocity = 14;
        [SerializeField] int raycastFrameInterval = 10;

        [SerializeField] AudioClip jumpSound = null;
        [SerializeField] Transform wallCheckUpper = null;
        [SerializeField] Transform wallCheckMiddle = null;

        //[Header("Attacks")]
        //[SerializeField] Vector2 forwardAttackSpeed = new Vector2(8, 0);
        //[SerializeField] Vector2 splashAttackSpeed = new Vector2(0, 0);

        [Header("SlopeManagement")]
        [SerializeField] float slopeCheckDistance = 0.5f;
        [SerializeField] PhysicsMaterial2D noFriction = null;
        [SerializeField] PhysicsMaterial2D lowFriction = null;
        [SerializeField] PhysicsMaterial2D fullFriction = null;
        [SerializeField] float maxSlopeAngle = 40;

        [Header("Evasion")]
        //[SerializeField] Vector2 simpleEvasionInitialVelocity = new Vector2(5,5);
        //[SerializeField] Vector2 dashSpeed = new Vector2(4,0);
        [SerializeField] AudioClip backJumpSound = null;
        [SerializeField] AudioClip rollsound = null;

        [Header("Extra")]
        [SerializeField] bool canDoubleJump = true;
        [SerializeField] ParticleSystem dust = null;

        #endregion

        public delegate void PlayerOnWall(bool state);
        public event PlayerOnWall OnPlayerWallState;

        PlayMakerFSM pm;
        GameObject oneWayPlatform;
        Rigidbody2D rb;
        Animator animator;
        AudioSource audioSource;
        PlayerHealth health;
        Vector2 slopeNormalPerp;
        Vector2 colliderSize;
        Vector2 finalPosition;
        float slopeDownAngle;
        float slopeDownAngleOld;
        float slopeSideAngle;
        float gravityScale;
        float ladderPositionX;
        bool isFalling = false;
        bool isLookingRight = true;
        bool isOnSlope = false;
        bool isGrounded;
        bool isJumping;
        bool canWalkOnSlope;
        bool canFlip = true;
        bool isCollidingWithOneWayPlatform = false;
        bool isPlayerOverLadder = false;
        bool canGetOffLadder = true;
        bool jumpButtonWasPressed = false;
        bool cr_running = false;
        bool isWallDetected = false;
        bool toolButtonState = false;
        //bool isJumpButtonPressed = false;


        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null) Debug.LogWarning("Rigidbody was not found");
            colliderSize = GetComponent<CapsuleCollider2D>().size;
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            health = GetComponent<PlayerHealth>();
        }

        void Start()
        {
            gravityScale = rb.gravityScale;
        }

         void FixedUpdate()
        {
            GroundCheck();
        }


        ////////////////////////////////////////////////////////////////////////////PUBLIC ///////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Used by all FSM in playmaker to set the current FSM to which all methods will refer to
        /// </summary>
        /// <param name="pm"></param>
        public void SetCurrentStateFSM(PlayMakerFSM pm)
        {
            this.pm = pm;
        }

        /// <summary>
        /// Do the movement of the player.
        /// It is called by the Movement State
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxSpeed"></param>
        public void ControlledMovement(Vector2 input, float maxSpeed, bool isGuarding)
        {
            animator.SetFloat("yVelocity", rb.velocity.y);
            if (!isGuarding)
            {
                ClimbingLedgeCheck();
                Flip(input.x);
            }
            SetMovement(input, maxSpeed, isGuarding);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xInput"></param>
        public void Flip(float xInput)
        {
            //The use of this method implies that you can Flip
            //If Flip is not allow please put that instruction outside this method
            Quaternion currentRotation = new Quaternion(0, 0, 0, 0);
            if (xInput > 0 && !isLookingRight)
            {
                CreateDust();
                //print("Change To Look Right");
                Vector3 rotation = new Vector3(0, 0, 0);
                currentRotation.eulerAngles = rotation;
                transform.rotation = currentRotation;
                isLookingRight = true;
            }
            else if (xInput < 0 && isLookingRight)
            {
                //print("Change To Look Left");
                Vector3 rotation = new Vector3(0, 180, 0);
                currentRotation.eulerAngles = rotation;
                transform.rotation = currentRotation;
                isLookingRight = false;
            }
        }

        /// <summary>
        /// Used by the Movement FSM to initialize the Climbing Edge State
        /// </summary>
        /// <param name="ledge"></param>
        public void ClimbingLedge(Vector2 ledge)
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
            rb.velocity = new Vector2(0, 0);
            animator.SetFloat("xVelocity", 0);
            animator.SetFloat("yVelocity", 0);
            GravityScale(0);
            Vector2 initialPosition = new Vector2(ledge.x + 0.1f * -transform.right.x, ledge.y - colliderSize.y);
            rb.MovePosition(initialPosition);
            animator.SetTrigger("ClimbLedge");
            finalPosition = new Vector2(ledge.x + 0.5f * transform.right.x, ledge.y + 0.00f);
        }

        /// <summary>
        /// Used by Stats in PlayMaker to know if the player can Jump
        /// </summary>
        /// <returns></returns>
        public bool CanJump()
        {
            if (isGrounded)
            {
                return true;
            }
            if (!isGrounded && canDoubleJump)
            {
                canDoubleJump = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Used by the Jump FSM to apply the jump force
        /// </summary>
        /// <param name="noFriction"></param>
        /// <param name="jumpSpeed"></param>
        public void ApplyJump(PhysicsMaterial2D noFriction, float jumpSpeed)
        {
            rb.gravityScale = gravityScale;
            rb.sharedMaterial = noFriction;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            animator.SetTrigger("Jump");
        }


        /// <summary>
        /// Check both if a ladder is detected and player input to start climbing the ladder.
        /// Used by the Movement State
        /// </summary>
        /// <param name="input"></param>
        public void CheckForLadder(Vector2 input)
        {
            if (isPlayerOverLadder)
            {
                CheckForClimbingLadder(input.y);
            }
        }


        /// <summary>
        /// Sets the player to climb the ladder.
        /// Used by the LadderClimbingState
        /// </summary>
        public void StartLadderMovement()
        {
            rb.velocity = new Vector2(0, 0);
            GravityScale(0);
            rb.position = new Vector3(ladderPositionX, transform.position.y, 0);
            canDoubleJump = true;
            StartCoroutine(TimerToGetOffLadder());
        }

        /// <summary>
        /// The controlled movement on the ladder
        /// Used by the LadderClimbingState
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxLadderMovementSpeed"></param>
        public void LadderMovement(Vector2 input, float maxLadderMovementSpeed)
        {
            //JumpCheck(input, true);
            if (cr_running) return;
            //print("Ladder Moving");
            animator.SetFloat("climbingSpeed", Mathf.Abs(input.y));
            rb.velocity = new Vector2(0, input.y * maxLadderMovementSpeed);
            if (canGetOffLadder) CheckToExitLadder(input.y);
        }

        public void DashMovement(Vector2 input, float maxSpeed)
        {
            Vector2 tempInput;
            if (isLookingRight)
                tempInput = new Vector2(1, 0);
            else
                tempInput = new Vector2(-1, 0);
            SlopeCheck(input.x);
            float currentSpeed = maxSpeed * tempInput.x;
            Move(currentSpeed);
        }

        public void SetPhysicsAttack(PhysicsMaterial2D fullFriction)
        {
            rb.gravityScale = gravityScale;
            rb.sharedMaterial = fullFriction;
        }

        //////////////////////////////////////////////////////////////////////////////////////PRIVATE/////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set the isGround, isJumping and canDoubleJump depending on GroundDetection
        /// </summary>
        void GroundCheck()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
            animator.SetBool("Grounded", isGrounded);
            if (isGrounded && canWalkOnSlope)
            {
                canDoubleJump = true;
            }
            if (isFalling && isGrounded)
            {
                //audioSource.PlayOneShot(landingSound);
            }
            if (rb.velocity.y <= 0.0f)
            {
                isJumping = false;
                isFalling = true;
            }
            if (isGrounded && !isJumping && canWalkOnSlope && rb.velocity.y == 0)
            {
                isFalling = false;

                isJumping = false;
            }
        }


        //Used by Moving State(From ControlledMovement)
        //Used by Guarding/Parrying
        void SetMovement(Vector2 input, float maxSpeed, bool isGuarding)
        {
            SlopeCheck(input.x);
            float currentSpeed = maxSpeed * input.x;
            if (isGuarding)
                animator.SetFloat("guardSpeed", Mathf.Abs(input.x));
            Move(currentSpeed);
        }

        void Move(float currentSpeed)
        {
            //print("Moving");
            if (isGrounded && !isOnSlope)
            {
                //print("Grounded not over slope");
                GroundedMoverNotOverSlope(currentSpeed);
            }
            else if (isGrounded && isOnSlope && canWalkOnSlope)
            {
                //print("Grounded over slope");
                GroundedMoveOverSlope(currentSpeed);
            }
            else if (!isGrounded)
            {
                //print("Not Grounded");
                MoveInAir(currentSpeed);
            }
            else
            {
                //Not Controlled Movement
            }
        }
        /// <summary>
        /// Grounded Movement without a Slope
        /// </summary>
        /// <param name="currentSpeed"></param>
        void MoveInAir(float currentSpeed)
        {
            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
            rb.sharedMaterial = noFriction;
            animator.SetFloat("xVelocity", Mathf.Abs(currentSpeed));
        }

        /// <summary>
        /// Grounded Movement with a Slope
        /// </summary>
        /// <param name="currentSpeed"></param>
        void GroundedMoveOverSlope(float currentSpeed)
        {
            float xVelocity = -1 * currentSpeed * slopeNormalPerp.x;
            float yVelocity = -1 * currentSpeed * slopeNormalPerp.y;
            rb.velocity = new Vector2(xVelocity, yVelocity);
            animator.SetFloat("xVelocity", Mathf.Abs(xVelocity));
        }

        /// <summary>
        /// Movement in the air
        /// </summary>
        /// <param name="currentSpeed"></param>
        void GroundedMoverNotOverSlope(float currentSpeed)
        {
            rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
            animator.SetFloat("xVelocity", Mathf.Abs(currentSpeed));
        }

        void CreateDust()
        {
            if (!isGrounded) return;
            dust.Play();
        }

        //AnimEvent
        void Footstep()
        {
            if (!isGrounded && footStepAudioSource.isPlaying)
                return;
            footStepAudioSource.pitch = Random.Range(0.75f, 1);
            footStepAudioSource.Play();
        }


        void ClimbingLedgeCheck()
        {
            RaycastHit2D isTouchingUpperWall;
            RaycastHit2D isTouchingMiddleWall;
            if (rb.velocity.y > 0)
            {
                //print("Ledge check Upwards");
                isTouchingMiddleWall = GetIsTouchingMiddleWall();
                if (!isTouchingMiddleWall) return;
                isTouchingUpperWall = GetIsTouchingUpperWall();
                if (isTouchingUpperWall) return;
            }
            else
            {
                //print("Ledge Check Downwards");
                isTouchingUpperWall = GetIsTouchingUpperWall();
                if (isTouchingUpperWall) return;
                isTouchingMiddleWall = GetIsTouchingMiddleWall();
                if (!isTouchingMiddleWall) return;
            }
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(transform.right.x, 2), Vector2.down, 2.5f, whatIsGround);
            print(hit.collider.name);
            Vector2 ledge = new Vector2(isTouchingMiddleWall.point.x, hit.point.y);
            //print("Start Climbing Ledge Movement");
            FsmEventData myfsmEventData = new FsmEventData();
            myfsmEventData.Vector2Data = ledge;
            HutongGames.PlayMaker.Fsm.EventData = myfsmEventData;
            pm.Fsm.Event("CLIMBINGLEDGE");
        }

        void SlopeCheck(float xInput)
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
            RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsGround);
            if (hit)
            {
                slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized;
                slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeDownAngle != slopeDownAngleOld)
                {
                    isOnSlope = true;
                    slopeDownAngleOld = slopeDownAngle;
                }
                Debug.DrawRay(hit.point, hit.normal, Color.green);
                Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
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

        //Anim Event
        void RollSound()
        {
            audioSource.PlayOneShot(rollsound);
        }

        //AnimEvent
        void BackjumpSound()
        {
            audioSource.PlayOneShot(backJumpSound);
        }
        /// <summary>
        /// Anim Event.
        /// </summary>
        void JumpSound()
        {
            audioSource.PlayOneShot(jumpSound);
        }

        public void MovementImpulse(float attackimpulse)
        {
            rb.velocity = new Vector2(transform.right.x * attackimpulse, rb.velocity.y);
        }

        RaycastHit2D GetIsTouchingUpperWall()
        {
            return Physics2D.Raycast(wallCheckUpper.position, transform.right, 0.7f, whatIsGround);
        }

        RaycastHit2D GetIsTouchingMiddleWall()
        {
            return Physics2D.Raycast(wallCheckMiddle.position, transform.right, 0.7f, whatIsGround);
        }
        //AnimEvent
        void FinishClimbLedge()
        {
            //print("Finish Ledge Climb");
            transform.position = finalPosition;
            GravityScale(gravityScale);
            gameObject.layer = LayerMask.NameToLayer("Player");
            pm.SendEvent("ACTIONFINISHED");
        }

        //AnimEvent
        void ClimbingLedgeSound()
        {
            climbingLegeAudioSource.Play();
        }

        /// <summary>
        /// Checks the player input to start climbing the ladder
        /// </summary>
        /// <param name="yInput"></param>
        void CheckForClimbingLadder(float yInput)
        {

            if (isCollidingWithOneWayPlatform && yInput < -0.9f)
            {
                oneWayPlatform.GetComponent<OneWayPlatform>().RotatePlatform();
                pm.SendEvent("LADDERCLIMB");
            }
            else if (!isGrounded && Mathf.Abs(yInput) > 0.9f)
            {
                pm.SendEvent("LADDERCLIMB");
            }

            else if (isGrounded && !isCollidingWithOneWayPlatform && yInput > 0.8f)
            {
                pm.SendEvent("LADDERCLIMB");
            }
        }


        /// <summary>
        /// A small safety timer to not allow the player exit the ladder instantely
        /// </summary>
        /// <returns></returns>
        IEnumerator TimerToGetOffLadder()
        {
            canGetOffLadder = false;
            yield return new WaitForSeconds(0.1f);
            canGetOffLadder = true;
        }

        /// <summary>
        /// The options for the player to exit the ladder without any additional input
        /// </summary>
        /// <param name="yInput"></param>
        void CheckToExitLadder(float yInput)
        {
            if (yInput > 0 && IsOneWayPlatformBeneathMe())
            {
                //print("ClimbingPlatform");
                FinishClimbingFromAbove();
            }

            else if (yInput < 0 && isGrounded && !isCollidingWithOneWayPlatform)
            {
                //print("Exiting Ladder from below");
                RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0,colliderSize.y/2), Vector2.down, checkLadderDistance, whatIsGround);
                if (!hit || hit.collider.CompareTag("ThinPlatform")) return;
                FinishClimbingFromBelow();
            }
        }


        /// <summary>
        /// Returns true if a One Way Platform is below the player 
        /// </summary>
        /// <returns></returns>
        bool IsOneWayPlatformBeneathMe()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, colliderSize.y / 2, whatIsLadderTop);
            
            if (hit && hit.collider.CompareTag("ThinPlatform"))
            {
                //print("Platform is below");
                return true;
            }
            else return false;
        }

        /// <summary>
        /// End Ladder Climbing using the Finish Climbing Ladder Animation
        /// </summary>
        void FinishClimbingFromAbove()
        {
            rb.velocity = new Vector2(0, 0);
            animator.SetInteger("LadderMovement", 5);
            StartCoroutine(CheckExitLadder());
            print("Exit Ladder from Above");

        }

        /// <summary>
        /// End Ladder Climbing going back to idle inmediately
        /// </summary>
        void FinishClimbingFromBelow()
        {
            //isMovingOnLadder = false;
            GravityScale(gravityScale);
            animator.SetInteger("LadderMovement", 10);
            pm.SendEvent("ACTIONFINISHED");
            print("Exit Ladder from Below");
        }

        /// <summary>
        /// Waiting for the animation to ends
        /// </summary>
        /// <returns></returns>
        IEnumerator CheckExitLadder()
        {
            cr_running = true;
            while(animator.GetInteger("LadderMovement")!= 100 && !health.IsDamaged())
            {
                print("Waiting to Exit Climbing Ladder");
                yield return null;
            }

            animator.SetInteger("LadderMovement", 0);

            GravityScale(gravityScale);
            pm.SendEvent("ACTIONFINISHED");
            print("Exit Ladder from Above.... Event sent to  " + pm.FsmName);
            cr_running = false;
        }

        //AnimEvent
        void ClimbingLadderSound()
        {
            audioSource.PlayOneShot(climbingLadderSound);
        }


        void GravityScale(float gravityScale)
        {
            rb.gravityScale = gravityScale;
        }


        void OnCollisionEnter2D(Collision2D collision)
        {
            //print("Collision Enter");
            if (collision.collider.CompareTag("ThinPlatform"))
            {
                isCollidingWithOneWayPlatform = true;
                oneWayPlatform = collision.collider.gameObject;
            }
        }

        void OnCollisionExit2D(Collision2D collision)
        {
            //print("Collision Enter");
            if (collision.collider.CompareTag("ThinPlatform"))
            {
                isCollidingWithOneWayPlatform = false;
                oneWayPlatform = null;
            }
        }


        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ladder")) 
            {
                //print("Ladder is detected");
                isPlayerOverLadder = true;
                ladderPositionX = collision.transform.position.x;
            }

            if (collision.CompareTag("VirtualCamera"))
            {
                collision.GetComponent<IChangeVCamera>().EnterCamArea();
            }
        }

        void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Ladder"))
            {
                //print("Ladder is not detected");
                isPlayerOverLadder = false;
                ladderPositionX = 0;
            }

            if (collision.CompareTag("VirtualCamera"))
            {
                collision.GetComponent<IChangeVCamera>().ExtiCamArea();
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }


    }

}
