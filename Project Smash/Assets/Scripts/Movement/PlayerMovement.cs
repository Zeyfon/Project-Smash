using PSmash.Attributes;
using Spine.Unity;
using System.Collections;
using UnityEngine;

namespace PSmash.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        [SpineBone(dataField: "skeletonRenderer")]
        [SerializeField] public string boneName;

        [Header("GroundCheck")]
        [SerializeField] LayerMask whatIsGround;
        [SerializeField] float groundCheckRadius = 0.1f;
        [SerializeField] Transform groundCheck1 = null;
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

        [Header("ClimbingEdges")]
        [SerializeField] LayerMask whatIsClimbable;


        [Header("Attacks")]
        [SerializeField] Vector2 forwardAttackSpeed = new Vector2(8, 0);
        [SerializeField] Vector2 splashAttackSpeed = new Vector2(0, 0);

        [Header("SlopeManagement")]
        [SerializeField] LayerMask whatIsSlope;
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


        public delegate void PlayerController(bool state);
        public static event PlayerController EnablePlayerController;
        public delegate void PlayerOnWall(bool state);
        public event PlayerOnWall OnPlayerWallState;

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
        bool lookingRight = true;
        bool isOnSlope = false;
        bool isGrounded;
        bool isJumping;
        bool canWalkOnSlope;
        bool isMovingOnLadder = false;
        bool isChargingAttack = false;
        bool isMovingOnWall = false;
        bool isEvading = false;
        bool isClimbingLedge = false;
        bool canFlip = true;
        bool isCollidingWithThinPlatform = false;
        bool moveThroughFloor = false;
        bool isLadderDetected = false;
        bool canGetOffLadder = true;
        bool isPlayerAboveLadderTop;

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

        // The ground will be checked all frame tu ensure the correct detection among all actions
        private void Update()
        {
            CheckGround();
            
        }

        //These methods are used by the player to control how the avatar will be moved. 
        //Depending on the circumstances is the movement type and values
        #region ControlledMovement Methods

        //This method is called all the time. 
        //As long as the player does not do anything that triggers the return 
        //in FixedUpdate in the PlayerController script
        public void ControlledMovement(float xInput, float yInput, bool isInteractingWithObject)
        {
            if (isEvading) return;
            if (isClimbingLedge)
            {
                print("climbing ledge");
                //rb.velocity = new Vector2(0, 0);
                return;
            }
            animator.SetFloat("yVelocity", rb.velocity.y);

            if (!isMovingOnLadder && !isMovingOnWall && canFlip)
            {
                CheckLedgeClimb();
            }

            if(!isMovingOnLadder && isLadderDetected)
            {
                CheckToStartLadderMovement(yInput);
            }

            if (isMovingOnWall)
            {
                WallMovement(xInput, yInput);
                return;
            }
            else if (isMovingOnLadder) 
            {
                //print("Ladder Movement");
                LadderMovement(yInput);
                return;
            }
            else
            {
                ConstantInputMovement(xInput);
                return;
            }
        }

        private void WallMovement(float xInput, float yInput)
        {
            //The gravity is set to 0 and to the initial value inside the IsMovingOnWall property
            animator.SetFloat("climbingSpeed",(Mathf.Sqrt(xInput * xInput + yInput * yInput)));
            rb.velocity = new Vector2(xInput, yInput) * maxWallSpeed;
        }

        void LadderMovement(float yInput)
        {
            //Debug.Log("Ladder Moving");
            animator.SetFloat("climbingSpeed", Mathf.Abs(yInput));
            rb.velocity = new Vector2(0, yInput * maxLadderMovementSpeed);
            //Debug.Log(rb.velocity);
            if(canGetOffLadder) LookingToExitLadder(yInput);
        }
        public void ConstantInputMovement(float xInput)
        {
            //Debug.Log(xInput);
            SlopeCheck(xInput);
            float currentSpeed;
            currentSpeed = SetSpeed(xInput);
            if (canFlip) Flip(xInput);
            MovementType(currentSpeed);
        }

        private float SetSpeed(float xInput)
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

        #endregion

        #region JumpControl
        public void Jump(float yInput)
        {
            if (isMovingOnWall)
            {
                print("Jumping Away From Wall");
                //Debug.Log("Jumps from a wall");
                canDoubleJump = true;
                IsMovingOnWall = false;
                ApplyingJump();
                GravityScale(gravityScale);
                rb.drag = 1;
            }
            //Jumping from the Ladder
            else if (isMovingOnLadder)
            {
                print("Jumping Away from Ladder");
                animator.SetInteger("LadderMovement", 10);
                canDoubleJump = true;
                ApplyingJump();
                GravityScale(gravityScale);
                rb.drag = 1;
                //canDetectLadder = false;
                isMovingOnLadder = false;
                StartCoroutine(LadderCheckStandBy());
            }
            else if (isCollidingWithThinPlatform && yInput < -0.5f)
            {
                moveThroughFloor = true;
            }
            // First Grounded Jump
            else if (isGrounded && canWalkOnSlope)
            {
                ApplyingJump();
            }
            //Second or inAirJump only one is allowed and will be reseted once 
            //The player is grounded
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

        IEnumerator LadderCheckStandBy()
        {
            yield return new WaitForSeconds(0.3f);
            //canDetectLadder = true;
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
            //The isGrounded check is because after a jump or fall the animations
            //pass first through the movement and that will produce always a footstep sound
            //regarless if it is grounded or not
            if (!isGrounded && footStepAudioSource.isPlaying) return;
            footStepAudioSource.pitch = Random.Range(0.75f, 1);
            footStepAudioSource.Play();
        }
        #endregion
        
        #region EvasiveMovement Control
        public void EvadeMovement(float xInput)
        {
            print("Wants to Dash");
            //Debug.Log("Wants to evade");
            if (xInput == 0)
            {
                //Back Jump Movement
                //Debug.Log("Simple jump behind evasion");
                StartCoroutine(CheckEvasion());
                animator.SetInteger("Evade", 1);
                rb.velocity = new Vector2(simpleEvasionInitialVelocity.x * -transform.right.x, simpleEvasionInitialVelocity.y);
            }
            else if(xInput != 0)
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

        void BackjumpSound()
        {
            audioSource.PlayOneShot(backJumpSound);
        }

        IEnumerator DashForceApplication()
        {
                while (true)
                {
                print("Roll force applying");
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
            if(coroutine != null) StopCoroutine(coroutine);
            isEvading = false;
            animator.SetInteger("Evade", 0);
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

    #endregion

        public void CheckGround()
        {
            isGrounded =  Physics2D.OverlapCircle(groundCheck1.position, groundCheckRadius, whatIsGround);
            animator.SetBool("Grounded", isGrounded);
            if(isFalling && isGrounded)
            {
               
                //audioSource.PlayOneShot(landingSound);
            }
            if (rb.velocity.y <= 0.0f)
            {
                isJumping = false;
                isFalling = true;
            }
            if (isGrounded && !isJumping && slopeDownAngle <= maxSlopeAngle)
            {
                isFalling = false;
                canDoubleJump = true;
                isJumping = false;
            }
        }

        #region ClimbingLedge
        void CheckLedgeClimb()
        {
            if (rb.velocity.y == 0) return;
            RaycastHit2D isTouchingMiddleWall;
            RaycastHit2D isTouchingUpperWall;
            //Check if the player is detecting floor in front of him
            //While moving upwards
            if (rb.velocity.y > 0)
            {
                //print("Ledge check Upwards");
                isTouchingMiddleWall = GetIsTouchingMiddleWall();
                if (!isTouchingMiddleWall) return;
                isTouchingUpperWall = GetIsTouchingUpperWall();
                if (isTouchingUpperWall) return;
            }
            //While moving downards
            else
            {
                
                //print("Ledge Check Downwards");
                isTouchingUpperWall = GetIsTouchingUpperWall();
                if (isTouchingUpperWall) return;
                isTouchingMiddleWall = GetIsTouchingMiddleWall();
                if (!isTouchingMiddleWall) return;
            }
            //The idea is to get first the upper corner of the ledge in a Vector2 component
            //From here we will set the player in a specific position to start the climbing ledge animation
            //The gravity will be set off and the player control will be disable during the animation
            //At the end of the animation the player transform will be set to the second point relative to the corner previously gotten
            Vector2 ledgeCorner = new Vector2(0, 0);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(transform.right.x, 2), Vector2.down, 2.5f, whatIsClimbable);
            print(hit.collider.name);
            ledgeCorner = new Vector2(isTouchingMiddleWall.point.x, hit.point.y);
            print("Start Climbing Ledge Movement");
            ClimbingLedge(ledgeCorner);
        }

        private RaycastHit2D GetIsTouchingUpperWall()
        {
            return Physics2D.Raycast(wallCheckUpper.position, transform.right, 0.7f, whatIsClimbable);
        }

        private RaycastHit2D GetIsTouchingMiddleWall()
        {
            return Physics2D.Raycast(wallCheckMiddle.position, transform.right, 0.7f, whatIsClimbable);
        }

        void ClimbingLedge(Vector2 ledgeCorner)
        {

            isClimbingLedge = true;
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
            isClimbingLedge = false;
        }

        //AnimEvent
        void ClimbingLedgeSound()
        {
            climbingLegeAudioSource.Play();
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
            RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsSlope);
            RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsSlope);
            Debug.DrawRay(checkPos, transform.right * slopeCheckDistance, Color.blue);
            Debug.DrawRay(checkPos, -transform.right * slopeCheckDistance, Color.blue);

            if (slopeHitFront && !slopeHitFront.collider.CompareTag("LadderTop") && Mathf.Abs(slopeHitFront.normal.x) < 0.9f)
            {
                isOnSlope = true;
                slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
            }
            else if (slopeHitBack &&!slopeHitBack.collider.CompareTag("LadderTop") && Mathf.Abs(slopeHitBack.normal.x) < 0.9f)
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
            RaycastHit2D hit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, whatIsSlope);
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

        #region LadderControl

        void CheckToStartLadderMovement(float yInput)
        {

            //Start at the upper part of the ladder
            if(isPlayerAboveLadderTop && isCollidingWithThinPlatform && yInput < -0.9f)
            {
                print("IsOverThinPlatform " + isCollidingWithThinPlatform + "  onMovingDownOnLadder  " + moveThroughFloor + "  isLadderDetected  " + isLadderDetected + "  isMovingOnLadder  " + isMovingOnLadder);
                print("Start At Upper Part of Ladder");
                moveThroughFloor = true;
                rb.velocity = new Vector2(0, 0);
                GravityScale(0);
                rb.position = new Vector3(ladderPositionX, transform.position.y, 0);
                isMovingOnLadder = true;
                animator.SetTrigger("climbLadder");
                StartCoroutine(TimerToGetOffLadder());
            }
            //Start ladder movement in the air
            else if(!isPlayerAboveLadderTop && !isGrounded && Mathf.Abs(yInput) > 0.9f)
            {
                print("Start at middle of Ladder");
                rb.velocity = new Vector2(0, 0);
                print(ladderPositionX +"  "+transform.position.y);
                GravityScale(0);
                rb.position = new Vector3(ladderPositionX, transform.position.y, 0);
                isMovingOnLadder = true;
                animator.SetTrigger("climbLadder");
                StartCoroutine(TimerToGetOffLadder());
            }
            //Start ladder movement at the bottom (Grounded)
            else if(isGrounded && !isCollidingWithThinPlatform && yInput > 0.8f)
            {
                print("Start at bottom of Ladder");
                GravityScale(0);
                rb.position = new Vector3(ladderPositionX, transform.position.y, 0);
                isMovingOnLadder = true;
                animator.SetTrigger("climbLadder");
                StartCoroutine(TimerToGetOffLadder());
            }
        }

        IEnumerator TimerToGetOffLadder()
        {
            canGetOffLadder = false;
            yield return new WaitForSeconds(0.1f);
            canGetOffLadder = true;
        }

        private void LookingToExitLadder(float yInput)
        {
            if (yInput > 0)
            {
                //Debug.Log("Checking for Above Ladder Exit");
                Debug.DrawRay(transform.position + new Vector3(0, colliderSize.y / 2f), Vector2.down, Color.green);
                RaycastHit2D[] hits = Physics2D.RaycastAll (transform.position + new Vector3(0, colliderSize.y / 2f), Vector2.down, checkLadderDistance, whatIsGround); 
                foreach(RaycastHit2D hit in hits)
                {
                    if (hit.collider.CompareTag("ThinPlatform"))
                    {
                        //Debug.Break();
                        print("Exiting Ladder from above");
                        Vector2 newPosition = new Vector2(hit.collider.transform.position.x, hit.collider.transform.position.y + hit.collider.GetComponent<BoxCollider2D>().size.y / 2 +0.1f);
                        print(newPosition);
                        ExitLadderFromAbove(newPosition);
                        return;
                    }
                }
            }

            else if (yInput < 0 && isGrounded && !isCollidingWithThinPlatform)
            {
                print("Exiting Ladder from below");
                RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0,colliderSize.y/2), Vector2.down, checkLadderDistance, whatIsGround);
                if (!hit || hit.collider.CompareTag("ThinPlatform")) return;
                ExitLadderFromBelow();
            }
        }

        void ExitLadderFromAbove(Vector3 newPosition)
        {
            rb.MovePosition(newPosition);
            animator.SetInteger("LadderMovement", 5);
            EnablePlayerController(false);
            rb.velocity = new Vector2(0, 0);
            isMovingOnLadder = false;
            moveThroughFloor = false;
            GravityScale(gravityScale);
            StartCoroutine(CheckExitLadder());
        }

        void ExitLadderFromBelow()
        {
            isMovingOnLadder = false;
            moveThroughFloor = false;
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
            isMovingOnLadder = false;
            moveThroughFloor = false;
            isClimbingLedge = false;
        }

        void ClimbingLadderSound()
        {
            audioSource.PlayOneShot(climbingLadderSound);
        }
        #endregion

        private void Flip(float xInput)
        {
            //The use of this method implies that you can Flip
            //If Flip is not allow please put that instruction outside this method
            Quaternion currentRotation = new Quaternion(0, 0, 0, 0);
            if (xInput > 0 && !lookingRight)
            {
                CreateDust();
                //print("Change To Look Right");
                Vector3 rotation = new Vector3(0, 0, 0);
                currentRotation.eulerAngles = rotation;
                transform.rotation = currentRotation;
                lookingRight = true;
            }
            if (xInput < 0 && lookingRight)
            {
                //print("Change To Look Left");
                CreateDust();
                Vector3 rotation = new Vector3(0, 180, 0);
                currentRotation.eulerAngles = rotation;
                transform.rotation = currentRotation;
                lookingRight = false;
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
            return isClimbingLedge;
        }

        public bool IsCollidingWithThinPlatform
        {
            get
            {
                return isCollidingWithThinPlatform;
            }
            set
            {
                isCollidingWithThinPlatform = value;
            }
        }

        public void SetIsLadderDetected(float ladderPositionX, bool isLadderDetected)
        {
            this.ladderPositionX = ladderPositionX;
            this.isLadderDetected = isLadderDetected;
        }

        public void IsPlayerAboveLadderTop(bool isPlayerAboveLadderTop)
        {
            this.isPlayerAboveLadderTop = isPlayerAboveLadderTop;
        }
        public bool MoveThroughFloor
        {
            get
            {
                return moveThroughFloor;
            }
            set
            {
                moveThroughFloor = value;
            }
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
            Gizmos.DrawWireSphere(groundCheck1.position, groundCheckRadius);
        }

    }

}
