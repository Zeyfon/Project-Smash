using HutongGames.PlayMaker;
using PSmash.Attributes;
using PSmash.SceneManagement;
using Spine.Unity;
using System.Collections;
using UnityEngine;
using GameDevTV.Saving;
using PSmash.Checkpoints;

namespace PSmash.Movement
{
    public class PlayerMovement : MonoBehaviour, ISaveable
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
        [SerializeField] float climbingLadderSpeedFactor = 0.4f;
        [SerializeField] float checkLadderDistance = 1;
        [SerializeField] LayerMask whatIsLadder;
        [SerializeField] LayerMask whatIsLadderTop;
        [SerializeField] Transform ladderCheck = null;
        [SerializeField] AudioClip climbingLadderSound = null;
        [SerializeField] AudioClip climbingWallSound = null;


        [Header("Movement")]
        [SerializeField] float baseSpeed = 7;
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

        FsmObject currentFSM;
        SlopeControl slope = new SlopeControl();
        //PlayMakerFSM pm;
        GameObject oneWayPlatform;
        Rigidbody2D rb;
        Animator animator;
        AudioSource audioSource;
        PlayerHealth health;
        Vector2 colliderSize;
        Vector2 finalPosition;

        Vector3 storedPosition;

        float gravityScale;
        float ladderPositionX;
        float jumpTimer = 0;
        bool isFalling = false;
        bool isLookingRight = true;
        bool isGrounded;
        bool isJumping;
        bool canWalkOnSlope;
        bool isCollidingWithOneWayPlatform = false;
        bool isPlayerOverLadder = false;
        bool canGetOffLadder = true;
        bool cr_running = false;
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
            currentFSM = FsmVariables.GlobalVariables.FindFsmObject("currentFSM");
            gravityScale = rb.gravityScale;
        }

         void FixedUpdate()
        {
            GroundCheck();
            SetVelocityInAnimator();
            jumpTimer += Time.deltaTime;
        }

        private void SetVelocityInAnimator()
        {

            //if (animator == null) return;
            float xVelocity = (transform.right.x * rb.velocity.x);// / baseSpeed;
            float yVelocity = rb.velocity.y;// / baseSpeed;
            animator.SetFloat("xVelocity", xVelocity);
            animator.SetFloat("yVelocity", yVelocity);

        }
        ////////////////////////////////////////////////////////////////////////////PUBLIC ///////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Used by all FSM in playmaker to set the current FSM to which all methods will refer to
        /// </summary>
        /// <param name="pm"></param>
        public void SetCurrentStateFSM(PlayMakerFSM pm)
        {
            //this.pm = pm;
        }

        /// <summary>
        /// Do the movement of the player.
        /// It is called by the Movement State
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxSpeed"></param>
        public void ControlledMovement(Vector2 input, float speedFactor, bool isGuarding)
        {
            if (!isGuarding)
            {
                ClimbingLedgeCheck();
                Flip(input.x);
            }
            SetMovement(input, speedFactor);
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
            jumpTimer = 0;
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

        public void RollMovement(float inputX, float speedFactor)
        {
            Vector2 input = new Vector2(inputX, 0);
            Move(input, speedFactor);
        }

        public void SetPhysicsAttack(PhysicsMaterial2D physicsMaterial)
        {
            rb.gravityScale = gravityScale;
            rb.sharedMaterial = physicsMaterial;
        }

        public void MovementImpulse(float attackimpulse)
        {
            Vector2 direction = slope.GetSlopeNormalPerp(transform.position, transform.right, slopeCheckDistance, maxSlopeAngle, whatIsGround);
            direction *= -1;
            rb.velocity = new Vector2(direction.x * attackimpulse, direction.y * attackimpulse);
        }

        //////////////////////////////////////////////////////////////////////////////////////PRIVATE/////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set the isGround, isJumping and canDoubleJump depending on GroundDetection
        /// </summary>
        void GroundCheck()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
            animator.SetBool("Grounded", isGrounded);
            canWalkOnSlope = slope.CanWalkOnSlope(transform.position, transform.right, slopeCheckDistance, maxSlopeAngle, whatIsGround);
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
        void SetMovement(Vector2 input, float speedFactor)
        {
            Move(input, speedFactor);
        }

        void Move(Vector2 input, float speedFactor)
        {
            if (isGrounded & jumpTimer >0.25f)
            {
                
                if (slope.IsOnSlope(transform.position, transform.right, slopeCheckDistance, whatIsGround))
                {
                    if (slope.CanWalkOnSlope(transform.position, transform.right, slopeCheckDistance, maxSlopeAngle, whatIsGround))
                    {
                        GroundMovement(input, speedFactor);
                    }
                    else
                    {
                        DoNotMove();
                    }
                }
                else
                {
                    GroundMovement(input, speedFactor);
                }
            }
            else
            {
                MoveInAir(input, speedFactor);
            }

        } 

        void GroundMovement(Vector2 input, float speedFactor)
        {
            //print("Grounded Movement");
            //print(" Input  " +input);
            rb.sharedMaterial = noFriction;
            if (input.magnitude == 0)
            {
                //print("input = 0");
                rb.sharedMaterial = lowFriction;
            }
            input.y = 0;
            Vector2 direction = slope.GetSlopeNormalPerp(transform.position, input, slopeCheckDistance, maxSlopeAngle, whatIsGround);
            //print("Direction  " + direction);
            if(direction.sqrMagnitude == 0)
            {
                MoveInAir(input, speedFactor);
                return;
            }
            float speed = baseSpeed * Mathf.Abs(input.x) * speedFactor;
            //print("Speed " + speed);
            float xVelocity = -1 * speed * direction.x;
            float yVelocity = -1 * speed * direction.y;
            rb.velocity = new Vector2(xVelocity, yVelocity);
        }

        void DoNotMove()
        {
            //print("Uncontrolled Movement");
            rb.sharedMaterial = noFriction;
        }

        /// <summary>
        /// Grounded Movement without a Slope
        /// </summary>
        /// <param name="speedFactor"></param>
        void MoveInAir(Vector2 input, float speedFactor)
        {
            //print("Not Grounded Movement");
            rb.velocity = new Vector2(input.x * speedFactor * baseSpeed, rb.velocity.y);
            rb.sharedMaterial = noFriction;
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
            (currentFSM.Value as PlayMakerFSM).SendEvent("CLIMBINGLEDGE");
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
            (currentFSM.Value as PlayMakerFSM).SendEvent("ACTIONFINISHED");
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
                (currentFSM.Value as PlayMakerFSM).SendEvent("LADDERCLIMB");
            }
            else if (!isGrounded && Mathf.Abs(yInput) > 0.9f)
            {
                (currentFSM.Value as PlayMakerFSM).SendEvent("LADDERCLIMB");
            }
            else if (isGrounded && !isCollidingWithOneWayPlatform && yInput > 0.8f)
            {
                (currentFSM.Value as PlayMakerFSM).SendEvent("LADDERCLIMB");
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
            (currentFSM.Value as PlayMakerFSM).SendEvent("ACTIONFINISHED");
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
            (currentFSM.Value as PlayMakerFSM).SendEvent("ACTIONFINISHED");
            print("Exit Ladder from Above.... Event sent to  " + (currentFSM.Value as PlayMakerFSM).FsmName);
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

        public object CaptureState()
        {
            PlayerPositionCheckpoint checkpoint = FindObjectOfType<PlayerPositionCheckpoint>();
            if (checkpoint != null && checkpoint.IsPlayerInSavePoint())
            {
                SerializableVector3 position = new SerializableVector3(transform.position);
                return position;
            }
            else
            {
                SerializableVector3 position = new SerializableVector3(storedPosition);
                return position;
            }
        }

        public void RestoreState(object state, bool isLoadLastScene)
        {
            SerializableVector3 position = (SerializableVector3)state;
            storedPosition = position.ToVector();

            if (isLoadLastScene)
            {
                transform.position = storedPosition;
            }
            //TODO
            //HOW TO DIFFERENTIATE 
            //WHEN IS A RESTORE FOR THE CHECKPOINT
            //WHEN FOR A SCENE TRANSITION
            //WHEN THE PLAYER DIED AND IS RESTORING FROM LAST SAVE POINT
        }
    }

}
