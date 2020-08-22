using System.Collections;
using UnityEngine;


namespace PSmash.Movement
{
    public class PlayerMovementV2 : MonoBehaviour
    {
        [Header("GroundCheck")]
        [SerializeField] LayerMask whatIsGround;
        [SerializeField] float groundCheckRadius = 0.1f;
        [SerializeField] Transform groundCheck1 = null;
        [SerializeField] Transform groundCheck2 = null;


        [Header("Ladder")]
        [SerializeField] float maxLadderMovementSpeed = 3;
        [SerializeField] float checkLadderDistance = 1;
        [SerializeField] LayerMask whatIsLadder;
        [SerializeField] LayerMask whatIsPlatform;
        [SerializeField] Transform ladderCheck = null;
        

        [Header("Movement")]
        [SerializeField] float maxRunningSpeed = 5;
        [SerializeField] float maxWallSpeed = 2;
        [SerializeField] float maxInteractingSpeed = 2;
        [SerializeField] float maxGuardingSpeed = 2;
        [SerializeField] float jumpVelocity = 14;
        [SerializeField] int raycastFrameInterval = 10;
        [SerializeField] AudioClip jumpSound;
        [SerializeField] Transform wallCheckUpper = null;
        [SerializeField] Transform wallCheckMiddle = null;


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
        [SerializeField] AudioClip dashSound = null;

        [Header("Extra")]
        [SerializeField] bool canDoubleJump = true;
        [SerializeField] ParticleSystem dust = null;

        Rigidbody2D rb;
        Animator animator;
        AudioSource audioSource;
        Coroutine coroutine;
        EventManager eventManager;
        Vector2 slopeNormalPerp;
        Vector2 colliderSize;
        Vector2 finalPosition;
        Vector2 playerCenter;
        float slopeDownAngle;
        float slopeDownAngleOld;
        float slopeSideAngle;
        float gravityScale;
        bool lookingRight = true;
        bool isOnSlope = false;
        bool isGrounded;
        bool isJumping;
        bool canWalkOnSlope;
        bool isMovingOnLadder = false;
        bool canUseLadder = true; //This is to know if the ladder movement is available or not
        bool isChargingAttack = false;
        bool isMovingOnWall = false;
        bool isEvading = false;
        bool canDashInAir = true;
        bool canDetectLadder = true;
        bool isClimbingLedge = false;
        bool canFlip = true;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            colliderSize = GetComponent<CapsuleCollider2D>().size;
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            eventManager = FindObjectOfType<EventManager>();
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
            if (isClimbingLedge) return;
            animator.SetFloat("yVelocity", rb.velocity.y);
            if (!isMovingOnLadder && canFlip) CheckLedgeClimb();
            if  (canFlip && !isMovingOnLadder && canUseLadder && canDetectLadder) 
            {
                LookingForLadder(yInput);
            } 

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
            LookingToExitLadder(yInput);
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
        public void Jump()
        {
            //Debug.Log("Wants to Jump");
            if (isMovingOnWall)
            {
                //Debug.Log("Jumps from a wall");
                IsMovingOnWall = false;
                canDoubleJump = true;
                ApplyingJump();
                GravityScale(gravityScale);
                rb.drag = 1;
                return;
            }
            if (isMovingOnLadder)
            {
                //Debug.Log("Jumps from a ladder");
                animator.SetInteger("LadderMovement", 10);
                canDoubleJump = true;
                ApplyingJump();
                canDetectLadder = false;
                isMovingOnLadder = false;
                GravityScale(gravityScale);
                rb.drag = 1;
                StartCoroutine(LadderCheckStandBy());
                return;
            }
            else if (isGrounded && canWalkOnSlope)
            {
                ApplyingJump();
                return;
            }
            else if (isOnSlope && canDoubleJump)
            {
                ApplyingJump();
                canDoubleJump = false;
                //Debug.Log("1 " + canDoubleJump);
                return;
            }
            else if (canDoubleJump && canWalkOnSlope)
            {
                ApplyingJump();
                canDoubleJump = false;
                //Debug.Log("2 " + canDoubleJump);
                return;
            }
        }

        IEnumerator LadderCheckStandBy()
        {
            yield return new WaitForSeconds(0.3f);
            canDetectLadder = true;
        }

        private void ApplyingJump()
        {
            isJumping = true;
            rb.velocity = Vector2.up * jumpVelocity;
            animator.SetTrigger("Jump");
        }

        #endregion
        
        #region EvasiveMovement Control
        public void EvadeMovement(float xInput)
        {
            //Debug.Log("Wants to evade");
            if (xInput == 0 && isGrounded)
            {
                //Back Jump Movement
                //Debug.Log("Simple jump behind evasion");
                StartCoroutine(CheckEvasion());
                animator.SetInteger("Evade", 1);
                rb.velocity = new Vector2(simpleEvasionInitialVelocity.x * -transform.right.x, simpleEvasionInitialVelocity.y);
            }
            else if(canDashInAir && xInput != 0)
            {
                //Frontal Dash Movement
                //Debug.Log("Will evade with a roll to the left");
                StartCoroutine(CheckEvasion());
                animator.SetInteger("Evade", 10);
                //animator.Play("Dash");
                coroutine = StartCoroutine(DashForceApplication());
            }
        }

        IEnumerator DashForceApplication()
        {
            if (isGrounded)
            {
                while (true)
                {
                    //Debug.Log("Evading");
                    ConstantInputMovement(transform.right.x);
                    yield return new WaitForFixedUpdate();
                }
            }
            else
            {
                canDashInAir = false;
                rb.gravityScale = 0;
                while (true)
                {
                    //Debug.Log("Evading");
                    rb.velocity = new Vector2(dashSpeed.x * transform.right.x, 0);
                    animator.SetFloat("xVelocity", Mathf.Abs(dashSpeed.x));
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        IEnumerator CheckEvasion()
        {
            isEvading = true;
            animator.SetInteger("Evade", 0);
            gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
            while (animator.GetInteger("Evade") != 100)
            {
                Debug.Log("InEvasion Coroutine");
                yield return new WaitForEndOfFrame();
            }
            isEvading = false;
            rb.gravityScale = gravityScale;
            animator.SetInteger("Evade", 0);
            gameObject.layer = LayerMask.NameToLayer("Player");
        }

    #endregion

        public void CheckGround()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck1.position, groundCheckRadius, whatIsGround);
            if (!isGrounded)
            {
                isGrounded = Physics2D.OverlapCircle(groundCheck2.position, groundCheckRadius, whatIsGround);
            }
            animator.SetBool("Grounded", isGrounded);
            //print("Is Grounded  "+ isGrounded);
            if (rb.velocity.y <= 0.0f)
            {
                isJumping = false;
            }
            if (isGrounded && !isJumping && slopeDownAngle <= maxSlopeAngle)
            {
                canDoubleJump = true;
                canDashInAir = true;
                isJumping = false;
            }
        }
        void CheckLedgeClimb()
        {
            RaycastHit2D isTouchingUpperWall = Physics2D.Raycast(wallCheckUpper.position, transform.right, 0.7f, whatIsGround);
            RaycastHit2D isTouchingMiddleWall = Physics2D.Raycast(wallCheckMiddle.position, transform.right, 0.7f, whatIsGround);

            if (!isTouchingUpperWall && isTouchingMiddleWall)
            {
                if (isTouchingMiddleWall.collider.CompareTag("LadderTop"))
                {
                    print("LadderTop seen");
                    return;
                }
                print(isTouchingMiddleWall.collider.name);
                //The idea is to get first the upper corner of the ledge in a Vector2 component
                //From here we will set the player in a specific position to start the climbing ledge animation
                //The gravity will be set off and the player control will be disable during the animation
                //At the end of the animation the player transform will be set to the second point relative to the corner previously gotten
                Vector2 ledgeCorner = new Vector2(0,0);
                RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(transform.right.x,0), Vector2.up, 1.5f, whatIsGround);
                BoxCollider2D colliderChecker = hit.collider.GetComponent<BoxCollider2D>();
                if(colliderChecker != null)
                {
                    ledgeCorner = new Vector2(isTouchingMiddleWall.point.x, colliderChecker.transform.position.y + colliderChecker.size.y / 2);
                }
                else if (hit)
                {
                    ledgeCorner = new Vector2(isTouchingMiddleWall.point.x, hit.point.y);
                    print(ledgeCorner);
                }
                ClimbingLedge(ledgeCorner);
            }
        }

        void ClimbingLedge(Vector2 ledgeCorner)
        {
            isClimbingLedge = true;
            eventManager.PlayerControlDisable();
            rb.gravityScale = 0;
            rb.velocity = new Vector2(0, 0);
            Vector2 initialPosition = new Vector2(ledgeCorner.x + 0.1f * -transform.right.x, ledgeCorner.y-colliderSize.y);
            rb.MovePosition(initialPosition);
            animator.SetTrigger("ClimbLedge");
            finalPosition = new Vector2(ledgeCorner.x + 0.5f * transform.right.x, ledgeCorner.y);          
        }
        //AnimEvent
        void FinishLedgeClimb()
        {
            transform.position = finalPosition;
            eventManager.PlayerControlEnable();
            rb.gravityScale = gravityScale;
            isClimbingLedge = false;
        }
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
            //if(slopeHitFront) print(slopeHitFront.normal);
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

        private void LookingForLadder(float yInput)
        {
            RaycastHit2D hit;
            if (yInput > 0.5f)
            {
                //Looking to go upwards
                //Debug.Log("Wants to go Up");
                hit = Physics2D.Raycast(transform.position , Vector2.up, checkLadderDistance, whatIsLadder); ;
                if (hit)
                {
                    StartLadderMovement(hit.transform,true);
                    return;
                }
            }

            else if (yInput < -0.5f)
            {
                //Looking to go downwards
                hit = Physics2D.Raycast(transform.position, Vector2.down, checkLadderDistance, whatIsGround);
                if (hit && hit.collider.CompareTag("Ground")) return;
                //Debug.Log("Wants to go Down");
                hit = Physics2D.Raycast(transform.position, Vector2.down, checkLadderDistance, whatIsLadder);
                if (hit)
                {
                    StartLadderMovement(hit.transform, false);
                    return;
                }
            }
        }

        void StartLadderMovement(Transform ladderTransform, bool movingUpwards)
        {
            rb.velocity = new Vector2(0, 0);
            rb.position = new Vector3(ladderTransform.position.x, transform.position.y, 0);
            isMovingOnLadder = true;
            GravityScale(0);
            if (!movingUpwards)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, checkLadderDistance, whatIsGround);
                if (hit && hit.collider.CompareTag("LadderTop"))
                {
                    hit.collider.transform.GetComponent<LadderTop>().InvertPlatform();
                    print(hit.collider.transform.position);
                    print(rb.position);
                }
            }
            animator.Play("ClimbingLadder");
        }

        private void LookingToExitLadder(float yInput)
        {
            RaycastHit2D hit;
            Vector3 raycastOriginPosition = transform.position + new Vector3(0, colliderSize.y / 2f);
            if (yInput > 0)
            {
                //Debug.Log("Checking for Above Ladder Exit");
                Debug.DrawRay(raycastOriginPosition, Vector2.up, Color.gray);
                hit = Physics2D.Raycast(raycastOriginPosition, Vector2.up, checkLadderDistance, whatIsLadder); ;
                if (!hit)
                {
                    print("Exiting Ladder from above");
                    ExitLadderFromAbove(hit.point);
                }
                return;
            }

            if (yInput < 0 && isGrounded)
            {
                hit = Physics2D.Raycast(transform.position + new Vector3(0,colliderSize.y/2), Vector2.down, checkLadderDistance, whatIsGround);
                if (!hit || hit.collider.CompareTag("LadderTop")) return;
                ExitLadderFromBelow();
            }
        }

        void ExitLadderFromAbove(Vector3 newPosition)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0,1f), Vector2.down, 2, whatIsGround);
            print(hit.collider.name + "  " + hit.point);
            rb.MovePosition(hit.point + new Vector2(0,0.05f));
            animator.SetInteger("LadderMovement", 5);
            eventManager.PlayerControlDisable();
            rb.velocity = new Vector2(0, 0);
            isMovingOnLadder = false;
            GravityScale(gravityScale);
            StartCoroutine(CheckExitLadder());
        }

        void ExitLadderFromBelow()
        {
            isMovingOnLadder = false;
            GravityScale(gravityScale);
            canDetectLadder = true;
            animator.SetInteger("LadderMovement", 10);
        }

        IEnumerator CheckExitLadder()
        {
            while(animator.GetInteger("LadderMovement")!= 100)
            {
                yield return new WaitForEndOfFrame();
            }
            animator.SetInteger("LadderMovement", 0);
            eventManager.PlayerControlEnable();
            canDetectLadder = true;
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

        //void CombatGravity() 
        //{
        //    rb.gravityScale = 0;
        //    rb.sharedMaterial = fullFriction;
        //    rb.velocity = new Vector2(0, 0);
        //}

        public void ResetGravity()
        {
            rb.gravityScale = gravityScale;
        }
        void GravityScale(float gravityScale)
        {
            rb.gravityScale = gravityScale;
        }
        public void SetVelocityTo0()
        {
            //Debug.Log("Player Stopped");
            if (coroutine != null) StopCoroutine(coroutine);
            rb.velocity = new Vector2(0, 0);
        }

        #region Status Properties

        public bool IsEvading
        {
            get
            {
                return isEvading;
            }
        }

        public bool IsGrounded
        {
            get
            {
                return isGrounded;
            }
        }
        public bool IsMovingOnLadder
        {
            get
            {
                return isMovingOnLadder;
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
                    rb.gravityScale = 0;
                    animator.SetInteger("WallMovement", 1);
                }
                else 
                { 
                    rb.gravityScale = gravityScale;
                    animator.SetInteger("WallMovement", 50);
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

        //AnimEvent
        void EvadeActionEffect(int sound)
        {
            switch (sound)
            {
                case 1:
                    audioSource.PlayOneShot(backJumpSound);
                    break;
                case 2:
                    audioSource.PlayOneShot(dashSound);
                    break;
            }

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

        void JumpSound()
        {
            audioSource.PlayOneShot(jumpSound);
        }

        #endregion

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(groundCheck1.position, groundCheckRadius);
            Gizmos.DrawWireSphere(groundCheck2.position, groundCheckRadius);
        }
    }

}
