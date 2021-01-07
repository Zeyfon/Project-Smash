﻿using HutongGames.PlayMaker;
using PSmash.Attributes;
using Spine.Unity;
using System.Collections;
using UnityEngine;

namespace PSmash.Movement
{
    public class PlayerMovement : MonoBehaviour
    {

        #region Inspector
        [SpineBone(dataField: "skeletonRenderer")]
        [SerializeField] public string boneName;

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

        public delegate void PlayerController(bool state);
        public static event PlayerController EnablePlayerController;
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
        bool isLadderDetected = false;
        bool canGetOffLadder = true;
        bool jumpButtonWasPressed = false;
        bool cr_running = false;
        bool isWallDetected = false;
        bool isThrowDaggerButtonJustPressed = false;
        bool wallMovementState = false;
        bool isJumpButtonPressed = false;


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

        public void SetCurrentStateFSM(PlayMakerFSM pm)
        {
            this.pm = pm;
            //print("Current State in Player is " + this.pm.FsmName);
        }

        void Start()
        {
            gravityScale = rb.gravityScale;
        }

        private void FixedUpdate()
        {
            GroundCheck();
            //print("IsClimbing  " + isClimbingLedge);
        }

        public void GroundCheck()
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
            if (isGrounded && !isJumping && canWalkOnSlope && rb.velocity.y ==0)
            {
                isFalling = false;

                isJumping = false;
            }
        }

        #region GeneralMovement Methods

        //This method is used by the MovingState FSM
        public void ControlledMovement(Vector2 input, float maxSpeed)
        {
            animator.SetFloat("yVelocity", rb.velocity.y);
            ClimbingLedgeCheck();
            Flip(input.x);
            ConstantInputMovement(input, maxSpeed, false);
        }

        //Used by Moving State(From ControlledMovement)
        //Used by Guarding/Parrying
        public void ConstantInputMovement(Vector2 input, float maxSpeed, bool isGuarding)
        {
            JumpCheck(input,false);
            SlopeCheck(input.x);
            if (isThrowDaggerButtonJustPressed)
                pm.SendEvent("THROWDAGGER");
            if (wallMovementState && isWallDetected)
                pm.SendEvent("WALLMOVEMENT");
            float currentSpeed = maxSpeed * input.x;
            if (isGuarding)
                animator.SetFloat("guardSpeed", Mathf.Abs(input.x));
            MovementType(currentSpeed);
        }

        void MovementType(float currentSpeed)
        {
            if (isGrounded && !isOnSlope && !isJumping)
            {
                //print("Grounded but not over a slope");
                rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
                animator.SetFloat("xVelocity", Mathf.Abs(currentSpeed));
            }
            else if (isGrounded && isOnSlope && !isJumping && canWalkOnSlope)
            {
                //print("Grounded and over a Slope");
                float xVelocity  = -1* currentSpeed * slopeNormalPerp.x;
                float yVelocity = -1 * currentSpeed * slopeNormalPerp.y;
                rb.velocity = new Vector2(xVelocity, yVelocity);
                animator.SetFloat("xVelocity", Mathf.Abs(xVelocity));
            }
            else if (!isGrounded)
            {
                //print("Not Grounded");
                rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
                rb.sharedMaterial = noFriction;
                animator.SetFloat("xVelocity", Mathf.Abs(currentSpeed));
            }
            else
            {
                print("NotMoving");
            }
        }

        public void ThrowDaggerButtonJustPressed(bool isThrowDaggerButtonJustPressed)
        {
            print("Wants to Throw a Dagger");
            this.isThrowDaggerButtonJustPressed = isThrowDaggerButtonJustPressed;
        }

        public void StopMovement()
        {
            rb.velocity = new Vector2(0, 0);
        }

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
            if (xInput < 0 && isLookingRight)
            {
                //print("Change To Look Left");
                CreateDust();
                Vector3 rotation = new Vector3(0, 180, 0);
                currentRotation.eulerAngles = rotation;
                transform.rotation = currentRotation;
                isLookingRight = false;
            }
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
        #endregion

        #region SlopeControl
        public void SlopeCheck(float xInput)
        {
            Vector2 checkPos = transform.position;// - new Vector3(0.0f, colliderSize.y / 2);
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
        #endregion

        #region WallMovement

        public bool CanMoveOnWall()
        {
            if (isWallDetected)
                return true;
            else
                return false;
        }

        public void SetWallMovementButtonPressed(bool wallMovementState)
        {
            this.wallMovementState = wallMovementState;
        }
        public void ResetGravity()
        {
            rb.gravityScale = gravityScale;
        }
        public void WallMovement(Vector2 input, float maxWallMovementSpeed)
        {
            JumpCheck(input, true);
            if (!isWallDetected)
                pm.SendEvent("TOOLACTION");
            canDoubleJump = true;
            //The gravity is set to 0 and to the initial value inside the IsMovingOnWall property
            animator.SetFloat("climbingSpeed", (Mathf.Sqrt(input.x * input.x + input.y * input.y)));
            rb.velocity = new Vector2(input.x, input.y) * maxWallMovementSpeed;
        }

        public void IsWallDetected(bool isWallDetected)
        {
            this.isWallDetected = isWallDetected;
            print("WallDetection " + this.isWallDetected);
        }

        //AnimEvent
        void ClimbingWallSound()
        {
            audioSource.PlayOneShot(climbingWallSound);
        }

        #endregion

        #region Jump
        private void JumpCheck(Vector2 input, bool isClimbing)
        {
            //print("Jump is being checked " + isJumpButtonPressed);
            if (isJumpButtonPressed)
            {
                if (isCollidingWithOneWayPlatform && input.y < -0.5f)
                    RotatePlatform();
                else if (CanJump(isClimbing))
                {
                    pm.SendEvent("JUMP");
                    print("Wants To Jump");
                }
            }
        }
        public bool CanJump(bool isClimbing)
        {

            if (isGrounded && canWalkOnSlope && rb.velocity.y < 0.5f)
            {
                print("Ground Jump");
                jumpButtonWasPressed = false;
                return true;
            }
            else if (jumpButtonWasPressed && canDoubleJump)
            {
                print("Mid Air Jump");
                if (!isClimbing)
                    canDoubleJump = false;
                jumpButtonWasPressed = false;
                return true;
            }
            return false;
        }

        public void SetJumpButtonPress()
        {
            //print("Jump Button was pressed");
            jumpButtonWasPressed = true;
        }

        public void SetJumpButtonState(bool isJumpButtonPressed)
        {
            this.isJumpButtonPressed = isJumpButtonPressed;
            //print(this.isJumpButtonPressed);
        }

        public void ApplyJump(PhysicsMaterial2D noFriction, float jumpSpeed)
        {
            rb.gravityScale = gravityScale;
            rb.sharedMaterial = noFriction;
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            animator.SetTrigger("Jump");
        }
        void JumpSound()
        {
            audioSource.PlayOneShot(jumpSound);
        }
        #endregion

        #region ClimbingLedge

        private void ClimbingLedgeCheck()
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
            print("Start Climbing Ledge Movement");
            FsmEventData myfsmEventData = new FsmEventData();
            myfsmEventData.Vector2Data = ledge;
            HutongGames.PlayMaker.Fsm.EventData = myfsmEventData;
            pm.Fsm.Event("CLIMBINGLEDGE");
        }

        private RaycastHit2D GetIsTouchingUpperWall()
        {
            return Physics2D.Raycast(wallCheckUpper.position, transform.right, 0.7f, whatIsGround);
        }

        private RaycastHit2D GetIsTouchingMiddleWall()
        {
            return Physics2D.Raycast(wallCheckMiddle.position, transform.right, 0.7f, whatIsGround);
        }

        public void ClimbingLedge(Vector2 ledge)
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
            rb.velocity = new Vector2(0, 0);
            animator.SetFloat("xVelocity", 0);
            animator.SetFloat("yVelocity", 0);
            GravityScale(0);
            Vector2 initialPosition = new Vector2(ledge.x + 0.1f * -transform.right.x, ledge.y-colliderSize.y);
            rb.MovePosition(initialPosition);
            animator.SetTrigger("ClimbLedge");
            finalPosition = new Vector2(ledge.x + 0.5f * transform.right.x, ledge.y+0.00f);          
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
        #endregion

        #region LadderMovement

        public void LadderMovementCheck(Vector2 input)
        {
            if (isLadderDetected)
            {
                LadderMovementStart(input.y);
            }
        }

        void LadderMovementStart(float yInput)
        {

            if (isCollidingWithOneWayPlatform && yInput < -0.9f)
            {
                //print("Start At Upper Part of Ladder");
                oneWayPlatform.GetComponent<OneWayPlatform>().RotatePlatform();
                pm.SendEvent("LADDERCLIMB");
                //StartLadderMovement();

            }
            else if (!isGrounded && Mathf.Abs(yInput) > 0.9f)
            {
                //print("Start at middle of Ladder");
                //StartLadderMovement();
                pm.SendEvent("LADDERCLIMB");

            }

            else if (isGrounded && !isCollidingWithOneWayPlatform && yInput > 0.8f)
            {
                //print("Start at bottom of Ladder");
                //StartLadderMovement();
                pm.SendEvent("LADDERCLIMB");

            }
        }

        public void StartLadderMovement()
        {
            //StartCoroutine(LadderMovementFixedUpdated());
            //print("Started LadderMovement");
            rb.velocity = new Vector2(0, 0);
            GravityScale(0);
            rb.position = new Vector3(ladderPositionX, transform.position.y, 0);
            canDoubleJump = true;
            //isMovingOnLadder = true;
            //animator.SetTrigger("climbLadder");
            StartCoroutine(TimerToGetOffLadder());
        }


        IEnumerator TimerToGetOffLadder()
        {
            canGetOffLadder = false;
            yield return new WaitForSeconds(0.1f);
            canGetOffLadder = true;
        }

        public void LadderMovement(Vector2 input, float maxLadderMovementSpeed)
        {
            JumpCheck(input, true);
            if (cr_running) return;
            //print("Ladder Moving");
            animator.SetFloat("climbingSpeed", Mathf.Abs(input.y));
            rb.velocity = new Vector2(0, input.y * maxLadderMovementSpeed);
            if (canGetOffLadder) LookingToExitLadder(input.y);
        }

        private void LookingToExitLadder(float yInput)
        {
            if (yInput > 0 && IsPlatformBelowMe())
            {
                //print("ClimbingPlatform");
                FinishClimbingLadderAnimation();
            }

            else if (yInput < 0 && isGrounded && !isCollidingWithOneWayPlatform)
            {
                //print("Exiting Ladder from below");
                RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0,colliderSize.y/2), Vector2.down, checkLadderDistance, whatIsGround);
                if (!hit || hit.collider.CompareTag("ThinPlatform")) return;
                ExitLadderFromBelow();
            }
        }



        bool IsPlatformBelowMe()
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, colliderSize.y / 2, whatIsLadderTop);
            
            if (hit && hit.collider.CompareTag("ThinPlatform"))
            {
                //print("Platform is below");
                return true;
            }

            else return false;
        }

        private void FinishClimbingLadderAnimation()
        {
            //isClimbingLedge = true;
            rb.velocity = new Vector2(0, 0);
            //print("Climbing Animation");
            animator.SetInteger("LadderMovement", 5);
            //EnablePlayerController(false);
            StartCoroutine(CheckExitLadder());
        }

        void ExitLadderFromBelow()
        {
            //isMovingOnLadder = false;
            GravityScale(gravityScale);
            animator.SetInteger("LadderMovement", 10);
            pm.SendEvent("ACTIONFINISHED");
            print("Exit Ladder from Below.... Event sent to  " + pm.FsmName);
        }

        IEnumerator CheckExitLadder()
        {
            cr_running = true;
            while(animator.GetInteger("LadderMovement")!= 100 && !health.IsDamaged())
            {
                yield return new WaitForEndOfFrame();
            }
            animator.SetInteger("LadderMovement", 0);

            GravityScale(gravityScale);
            pm.SendEvent("ACTIONFINISHED");
            print("Exit Ladder from Above.... Event sent to  " + pm.FsmName);
            cr_running = false;
            //EnablePlayerController(true);
            //isMovingOnLadder = false;
            //isClimbingLedge = false;
        }

        //AnimEvent
        void ClimbingLadderSound()
        {
            audioSource.PlayOneShot(climbingLadderSound);
        }
        #endregion

        #region Rolling
        public void DashMovement(Vector2 input, float maxSpeed)
        {
            Vector2 tempInput;
            if (isLookingRight)
                tempInput = new Vector2(1, 0);
            else
                tempInput = new Vector2(-1, 0);
            JumpCheck(input, false);
            SlopeCheck(input.x);
            if (isThrowDaggerButtonJustPressed)
                pm.SendEvent("THROWDAGGER");
            float currentSpeed = maxSpeed * tempInput.x;
            MovementType(currentSpeed);
        }
        #endregion
        public void SetPhysicsAttack( PhysicsMaterial2D fullFriction)
        {
            rb.gravityScale = gravityScale;
            rb.sharedMaterial = fullFriction;
        }

        #region PlatformRotation
        public GameObject CanGoDownThroughOneWayPlaform(float yInput, bool jumpState)
        {
            //print("Checking one way platforms");
            if (jumpState && isCollidingWithOneWayPlatform && yInput < -0.5f) return oneWayPlatform;
            else return null;
        }

        public void RotatePlatform()
        {
            oneWayPlatform.GetComponent<OneWayPlatform>().RotatePlatform();
        }
        #endregion




        void GravityScale(float gravityScale)
        {
            rb.gravityScale = gravityScale;
        }

        #region Status Properties

        public bool GetIsLookingRight()
        {
            return isLookingRight;
        }

        public bool CanFlip
        {
            set
            {
                canFlip = value;
            }
        }

        #endregion

        #region Events
        //AnimEvent

        #endregion


        private void OnCollisionEnter2D(Collision2D collision)
        {
            //print("Collision Enter");
            if (!collision.collider.CompareTag("ThinPlatform")) return;

            isCollidingWithOneWayPlatform = true;
            oneWayPlatform = collision.collider.gameObject;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            //print("Collision Enter");
            if (!collision.collider.CompareTag("ThinPlatform")) return;

            isCollidingWithOneWayPlatform = false;
            oneWayPlatform = null;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ladder")) 
            {
                //print("Ladder is detected");
                isLadderDetected = true;
                ladderPositionX = collision.transform.position.x;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Ladder"))
            {
                //print("Ladder is not detected");
                isLadderDetected = false;
                ladderPositionX = 0;
            }
        }



        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }

}
