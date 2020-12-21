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
        [SerializeField] float maxLadderMovementSpeed = 3;
        [SerializeField] float checkLadderDistance = 1;
        [SerializeField] LayerMask whatIsLadder;
        [SerializeField] LayerMask whatIsLadderTop;
        [SerializeField] Transform ladderCheck = null;
        [SerializeField] AudioClip climbingLadderSound = null;
        [SerializeField] AudioClip climbingWallSound = null;
        

        [Header("Movement")]
        [SerializeField] float maxRunningSpeed = 5;
        [SerializeField] float maxWallSpeed = 2;
        [SerializeField] float maxInteractingSpeed = 2;
        [SerializeField] float maxGuardingSpeed = 2;
        [SerializeField] float jumpVelocity = 14;
        [SerializeField] int raycastFrameInterval = 10;
        [SerializeField] AudioClip jumpSound = null;
        [SerializeField] Transform wallCheckUpper = null;
        [SerializeField] Transform wallCheckMiddle = null;

        [Header("Attacks")]
        [SerializeField] Vector2 forwardAttackSpeed = new Vector2(8, 0);
        [SerializeField] Vector2 splashAttackSpeed = new Vector2(0, 0);

        [Header("SlopeManagement")]
        [SerializeField] float slopeCheckDistance = 0.5f;
        [SerializeField] PhysicsMaterial2D noFriction = null;
        [SerializeField] PhysicsMaterial2D lowFriction = null;
        [SerializeField] PhysicsMaterial2D fullFriction = null;
        [SerializeField] float maxSlopeAngle = 40;

        [Header("Evasion")]
        [SerializeField] Vector2 simpleEvasionInitialVelocity = new Vector2(5,5);
        [SerializeField] Vector2 dashSpeed = new Vector2(4,0);
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

        Transform oneWayPlatform;
        Rigidbody2D rb;
        Animator animator;
        AudioSource audioSource;
        Coroutine coroutine;
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
        bool isMovingOnLadder = false;
        bool isMovingOnWall = false;
        bool isEvading = false;
        bool isClimbing = false;
        bool canFlip = true;
        bool isCollidingWithOneWayPlatform = false;
        bool isLadderDetected = false;
        bool canGetOffLadder = true;
        bool canPassPlatformFromBelow = false;

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

        private void FixedUpdate()
        {
            GroundCheck();
        }

        public void GroundCheck()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
            animator.SetBool("Grounded", isGrounded);
            if (isFalling && isGrounded)
            {
                //audioSource.PlayOneShot(landingSound);
            }
            if (rb.velocity.y <= 0.0f)
            {
                isJumping = false;
                isFalling = true;
            }
            if (isGrounded && !isJumping && slopeDownAngle <= maxSlopeAngle && rb.velocity.y ==0)
            {
                isFalling = false;
                canDoubleJump = true;
                isJumping = false;
            }
        }

        #region GeneralMovement Methods

        //This method is called all the time. 
        //As long as the player does not do anything that triggers the return 
        //in FixedUpdate in the PlayerController script
        public void ControlledMovement(float xInput, float yInput, bool jump)
        {
            if (isEvading) return;
            if (jump)
            {
                if (isMovingOnWall)
                {
                    //print("Jumping Away From Wall");
                    canDoubleJump = true;
                    IsMovingOnWall = false;
                    ApplyingJump();
                    GravityScale(gravityScale);
                    rb.drag = 1;
                }
                //Jumping from the Ladder
                else if (isMovingOnLadder)
                {
                    //print("Jumping Away from Ladder");
                    canDoubleJump = true;
                    isMovingOnLadder = false;
                    ApplyingJump();
                    GravityScale(gravityScale);
                    rb.drag = 1;
                    animator.SetInteger("LadderMovement", 10);
                    StartCoroutine(LadderCheckStandBy());
                }
                else if (CanPassThroughOneWayPlatform(yInput))
                {
                    //print("Passing through one way platform");
                    oneWayPlatform.GetComponent<OneWayPlatform>().RotatePlatform();
                }

                else if (isGrounded && canWalkOnSlope)
                {
                    ApplyingJump();
                }

                else if (isOnSlope && canDoubleJump)
                {
                    ApplyingJump();
                    canDoubleJump = false;
                }
                else if (canDoubleJump && canWalkOnSlope)
                {
                    ApplyingJump();
                    canDoubleJump = false;
                }
            }

            if (isClimbing) return;

            animator.SetFloat("yVelocity", rb.velocity.y);
            if (!isMovingOnLadder && !isMovingOnWall && canFlip)
            {
                CheckForInteractableElements(yInput);
            }

            Moving(xInput, yInput);
        }

        private bool CanPassThroughOneWayPlatform(float yInput)
        {
            return isCollidingWithOneWayPlatform && yInput < -0.5f;
        }

        private void Moving(float xInput, float yInput)
        {
            if (isMovingOnWall)
            {
                WallMovement(xInput, yInput);
                return;
            }
            else if (isMovingOnLadder)
            {
                LadderMovement(yInput);
                return;
            }
            else
            {
                ConstantInputMovement(xInput);
                return;
            }
        }

        void WallMovement(float xInput, float yInput)
        {
            //The gravity is set to 0 and to the initial value inside the IsMovingOnWall property
            animator.SetFloat("climbingSpeed",(Mathf.Sqrt(xInput * xInput + yInput * yInput)));
            rb.velocity = new Vector2(xInput, yInput) * maxWallSpeed;
        }

        void LadderMovement(float yInput)
        {
            //print("Ladder Moving");
            animator.SetFloat("climbingSpeed", Mathf.Abs(yInput));
            rb.velocity = new Vector2(0, yInput * maxLadderMovementSpeed);
            if(canGetOffLadder) LookingToExitLadder(yInput);
        }
        void ConstantInputMovement(float xInput)
        {
            //print(xInput);
            SlopeCheck(xInput);
            float currentSpeed;
            currentSpeed = SetSpeed(xInput);
            if (canFlip) Flip(xInput);
            MovementType(currentSpeed);
        }

        float SetSpeed(float xInput)
        {
            float currentSpeed;
            if (!canFlip)
            {
                currentSpeed = xInput * maxGuardingSpeed;
            }
            else if (isEvading == true)
            {
                currentSpeed = xInput * dashSpeed.x;
            }
            else
            {
                currentSpeed = xInput * maxRunningSpeed;
            }
            return currentSpeed;
        }

        void MovementType(float currentSpeed)
        {
            if (!canFlip) animator.SetFloat("guardSpeed", Mathf.Abs(currentSpeed / maxGuardingSpeed));
            //Debug.Log(currentSpeed);
            if (isGrounded && !isOnSlope && !isJumping)
            {
                //Movement on Slopes
                //print("Not In Slope");
                rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
                animator.SetFloat("xVelocity", Mathf.Abs(currentSpeed));
                return;
            }
            if (isGrounded && isOnSlope && !isJumping && canWalkOnSlope)
            {
                //Movement in Slopes
                //print("In Slope");
                float xVelocity  = -1* currentSpeed * slopeNormalPerp.x;
                float yVelocity = -1 * currentSpeed * slopeNormalPerp.y;
                rb.velocity = new Vector2(xVelocity, yVelocity);
                //Debug.Log(rb.velocity + "  " + slopeNormalPerp);
                animator.SetFloat("xVelocity", Mathf.Abs(xVelocity));
                return;
            }
            if (!isGrounded)
            {
                // AirMovement
                //Debug.Log("In The Air");
                rb.velocity = new Vector2(currentSpeed, rb.velocity.y);
                rb.sharedMaterial = noFriction;
                animator.SetFloat("xVelocity", Mathf.Abs(currentSpeed));
                return;
            }
        }

        public void StopMovement()
        {
            rb.velocity = new Vector2(0, 0);
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

        IEnumerator LadderCheckStandBy()
        {
            yield return new WaitForSeconds(0.3f);
        }

        private void ApplyingJump()
        {
            isJumping = true;
            rb.sharedMaterial = noFriction;
            rb.velocity = Vector2.up * jumpVelocity;
            animator.SetTrigger("Jump");
        }

        //AnimEvent
        void Footstep()
        {
            if (!isGrounded && footStepAudioSource.isPlaying) return;
            footStepAudioSource.pitch = Random.Range(0.75f, 1);
            footStepAudioSource.Play();
        }

        #region ClimbingLedge
        void CheckForInteractableElements(float yInput)
        {
            if (isLadderDetected)
            {
                CheckToStartMovingOnLadders(yInput);
            }
            else
            {
                ClimbCorners();
            }
        }

        void CheckToStartMovingOnLadders(float yInput)
        {
            //print(canPassPlatformFromBelow + " " +  isCollidingWithOneWayPlatform);
            if (!canPassPlatformFromBelow && isCollidingWithOneWayPlatform && yInput < -0.9f)
            {
                //print("Start At Upper Part of Ladder");
                oneWayPlatform.GetComponent<OneWayPlatform>().RotatePlatform();
                StartLadderMovement();

            }
            else if (!isGrounded && Mathf.Abs(yInput) > 0.9f)
            {
                //print("Start at middle of Ladder");
                StartLadderMovement();
            }

            else if (isGrounded && !isCollidingWithOneWayPlatform && yInput > 0.8f)
            {
                //print("Start at bottom of Ladder");
                StartLadderMovement();
            }
        }

        private void StartLadderMovement()
        {
            //StartCoroutine(LadderMovementFixedUpdated());
            print("Started LadderMovement");
            rb.velocity = new Vector2(0, 0);
            GravityScale(0);
            rb.position = new Vector3(ladderPositionX, transform.position.y, 0);
            isMovingOnLadder = true;
            animator.SetTrigger("climbLadder");
            StartCoroutine(TimerToGetOffLadder());
        }

        IEnumerator LadderMovementFixedUpdated()
        {
            yield return new WaitForFixedUpdate();
            print("Started LadderMovement");
            rb.velocity = new Vector2(0, 0);
            GravityScale(0);
            rb.position = new Vector3(ladderPositionX, transform.position.y, 0);
            isMovingOnLadder = true;
            animator.SetTrigger("climbLadder");
            yield return TimerToGetOffLadder();
        }
        private void ClimbingPlatformAnimation()
        {
            isClimbing = true;
            rb.velocity = new Vector2(0, 0);
            print("Climbing Animation");
            animator.SetInteger("LadderMovement", 5);
            EnablePlayerController(false);
            StartCoroutine(CheckExitLadder());
        }

        private void ClimbCorners()
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
            Vector2 ledgeCorner = new Vector2(isTouchingMiddleWall.point.x, hit.point.y);
            print("Start Climbing Ledge Movement");
            ClimbingLedge(ledgeCorner);
        }

        private RaycastHit2D GetIsTouchingUpperWall()
        {
            return Physics2D.Raycast(wallCheckUpper.position, transform.right, 0.7f, whatIsGround);
        }

        private RaycastHit2D GetIsTouchingMiddleWall()
        {
            return Physics2D.Raycast(wallCheckMiddle.position, transform.right, 0.7f, whatIsGround);
        }

        void ClimbingLedge(Vector2 ledgeCorner)
        {

            isClimbing = true;
            rb.velocity = new Vector2(0, 0);
            EnablePlayerController(false);
            animator.SetFloat("xVelocity", 0);
            animator.SetFloat("yVelocity", 0);
            GravityScale(0);
            Vector2 initialPosition = new Vector2(ledgeCorner.x + 0.1f * -transform.right.x, ledgeCorner.y-colliderSize.y);
            rb.MovePosition(initialPosition);
            //Debug.Break();
            animator.SetTrigger("ClimbLedge");
            finalPosition = new Vector2(ledgeCorner.x + 0.5f * transform.right.x, ledgeCorner.y+0.00f);          
        }
        //AnimEvent
        void FinishClimbLedge()
        {
            print("Finish Ledge Climb");
            transform.position = finalPosition;
            //Debug.Break();
            EnablePlayerController(true);
            GravityScale(gravityScale);
            isClimbing = false;
        }

        //AnimEvent
        void ClimbingLedgeSound()
        {
            climbingLegeAudioSource.Play();
        }
        #endregion

        #region LadderControl

        IEnumerator TimerToGetOffLadder()
        {
            canGetOffLadder = false;
            yield return new WaitForSeconds(0.1f);
            canGetOffLadder = true;
        }

        private void LookingToExitLadder(float yInput)
        {
            if (yInput > 0 && IsPlatformBelowMe())
            {
                print("ClimbingPlatform");
                ClimbingPlatformAnimation();
            }

            else if (yInput < 0 && isGrounded && !isCollidingWithOneWayPlatform)
            {
                print("Exiting Ladder from below");
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
                print("Platform is below");
                return true;
            }

            else return false;
        }

        void ExitLadderFromBelow()
        {
            isMovingOnLadder = false;
            GravityScale(gravityScale);
            animator.SetInteger("LadderMovement", 10);
        }

        IEnumerator CheckExitLadder()
        {
            while(animator.GetInteger("LadderMovement")!= 100 && !health.IsDamaged())
            {
                yield return new WaitForEndOfFrame();
            }
            animator.SetInteger("LadderMovement", 0);
            EnablePlayerController(true);
            GravityScale(gravityScale);
            isMovingOnLadder = false;
            isClimbing = false;
        }

        //AnimEvent
        void ClimbingLadderSound()
        {
            audioSource.PlayOneShot(climbingLadderSound);
        }
        #endregion

        #region EvasiveMovement Control
        public void EvadeMovement(float xInput)
        {
            //print("Wants to Dash");
            //Debug.Log("Wants to evade");
            if (xInput == 0)
            {
                //Back Jump Movement
                //Debug.Log("Simple jump behind evasion");
                StartCoroutine(CheckEvasion());
                animator.SetInteger("Evade", 1);
                rb.velocity = new Vector2(simpleEvasionInitialVelocity.x * -transform.right.x, simpleEvasionInitialVelocity.y);
            }
            else if (xInput != 0)
            {
                //Frontal Dash Movement
                //Debug.Log("Will evade with a roll to the left");
                StartCoroutine(CheckEvasion());
                animator.SetInteger("Evade", 10);
                //animator.Play("Dash");
                coroutine = StartCoroutine(DashForceApplication());
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

        IEnumerator DashForceApplication()
        {
            while (true)
            {
                //print("Roll force applying");
                ConstantInputMovement(transform.right.x);
                animator.SetFloat("yVelocity", rb.velocity.y);
                yield return new WaitForFixedUpdate();
            }
        }

        IEnumerator CheckEvasion()
        {
            isEvading = true;
            animator.SetInteger("Evade", 0);
            gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
            while (animator.GetInteger("Evade") != 100 && !health.IsDamaged())
            {
                //Debug.Log("InEvasion Coroutine");
                yield return new WaitForEndOfFrame();
            }
            if (coroutine != null) StopCoroutine(coroutine);
            isEvading = false;
            animator.SetInteger("Evade", 0);
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

        #endregion

        private void Flip(float xInput)
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

        public void ResetGravity()
        {
            GravityScale(gravityScale);
        }
        void GravityScale(float gravityScale)
        {
            rb.gravityScale = gravityScale;
        }
        public void SetVelocityToCero()
        {
            //Debug.Log("Player Stopped");
            if (coroutine != null) StopCoroutine(coroutine);
            rb.velocity = new Vector2(0, 0);
        }

        #region Status Properties


        public bool IsEvading()
        {
            return isEvading;
        }
        public bool IsGrounded()
        {
            return isGrounded;
        }

        public bool IsMovingOnLadder()
        {
            return isMovingOnLadder;
        }

        public bool IsClimbingLedge()
        {
            return isClimbing;
        }

        public bool IsMovingOnWall
        {
            get
            {
                return isMovingOnWall;
            }
            set
            {
                isMovingOnWall = value;
                if (isMovingOnWall)
                {
                    if (transform.right.x < 0) Flip(1);
                    GravityScale(0);
                    animator.SetInteger("WallMovement", 1);
                    if (OnPlayerWallState == null) return;
                    OnPlayerWallState(true);
                }
                else 
                { 
                    GravityScale(gravityScale);
                    animator.SetInteger("WallMovement", 50);
                    if (OnPlayerWallState == null) return;
                    OnPlayerWallState(false);
                }

            }
        }

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
        void CreateDust()
        {
            if (!isGrounded) return;
            dust.Play();
        }

        //Used by the PlayerFighter to apply an small impulse when attacking
        public void AddImpulse(float impulse)
        {
            rb.AddForce(new Vector2(impulse * transform.right.x, 0),ForceMode2D.Impulse);
        }

        void ForwadAttackMovement()
        {
            coroutine = StartCoroutine(ForwardAttackMovementCO());
        }

        IEnumerator ForwardAttackMovementCO()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                ControlledMovement(forwardAttackSpeed.x*transform.right.x, forwardAttackSpeed.y, false);
            }
        }
        IEnumerator SplashMovement()
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
            while (!isGrounded)
            {
                rb.velocity = splashAttackSpeed;
                yield return new WaitForFixedUpdate();
            }
            rb.velocity = new Vector2(0, 0);
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

        //AnimEvent
        void JumpSound()
        {
            audioSource.PlayOneShot(jumpSound);
        }

        //AnimEvent
        void ClimbingWallSound()
        {
            audioSource.PlayOneShot(climbingWallSound);
        }
        #endregion

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            //print("Collision Enter");
            if (!collision.collider.CompareTag("ThinPlatform")) return;

            isCollidingWithOneWayPlatform = true;
            oneWayPlatform = collision.collider.transform;
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            //print("Collision Enter");
            if (!collision.collider.CompareTag("ThinPlatform")) return;
            if (collision.collider.transform.position.y < transform.position.y)
            {
                canPassPlatformFromBelow = false;
                print("Player is Above");
            }
            else
            {
                print(collision.collider.transform.position.y + " " + transform.position.y);
                canPassPlatformFromBelow = true;
                print("Player is Below");
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            //print("Collision Enter");
            if (!collision.collider.CompareTag("ThinPlatform")) return;

            isCollidingWithOneWayPlatform = false;
            canPassPlatformFromBelow = false;
            oneWayPlatform = null;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Ladder")) 
            {
                print("Ladder is detected");
                isLadderDetected = true;
                ladderPositionX = collision.transform.position.x;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Ladder"))
            {
                print("Ladder is not detected");

                isLadderDetected = false;
                ladderPositionX = 0;
            }
        }
    }

}
