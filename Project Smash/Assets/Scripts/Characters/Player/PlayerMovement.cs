using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using PSmash.Combat;

namespace PSmash.Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("GroundCheck")]
        [SerializeField] LayerMask whatIsGround;
        [SerializeField] float groundCheckRadius = 0.5f;
        [SerializeField] Transform groundCheck1 = null;
        [SerializeField] Transform groundCheck2 = null;

        [Header("Ladder")]
        [SerializeField] float checkLadderDistance = 1;
        [SerializeField] LayerMask whatIsLadder;
        [SerializeField] LayerMask whatIsPlatform;
        [SerializeField] Transform ladderCheck = null;
        

        [Header("Movement")]
        [SerializeField] float maxRunningSpeed = 5;
        [SerializeField] float maxWallSpeed = 2;
        [SerializeField] float maxInteractingSpeed = 2;
        [SerializeField] float jumpVelocity = 8;
        [SerializeField] int raycastFrameInterval = 10;
        [SerializeField] AudioClip jumpSound;

        [Header("Attacks")]
        //[SerializeField] Vector2 splashAttackSpeed = new Vector2(0,0);
        [SerializeField] Vector2 forwardAttackSpeed = new Vector2(0, 0);

        [Header("SlopeManagement")]
        [SerializeField] LayerMask whatIsSlope;
        [SerializeField] float slopeCheckDistance = 0;
        [SerializeField] PhysicsMaterial2D noFriction = null;
        [SerializeField] PhysicsMaterial2D lowFriction = null;
        [SerializeField] PhysicsMaterial2D fullFriction = null;
        [SerializeField] float maxSlopeAngle = 0;

        [Header("Evasion")]
        [SerializeField] Vector2 simpleEvasionInitialVelocity;
        [SerializeField] Vector2 rollSpeed = new Vector2(0,0);

        [Header("Extra")]
        [SerializeField] bool canDoubleJump = true;
        [SerializeField] ParticleSystem dust = null;

        Rigidbody2D rb;
        Animator animator;
        AudioSource audioSource;
        Coroutine coroutine;
        Vector2 slopeNormalPerp;
        Vector2 colliderSize;
        float slopeDownAngle;
        float slopeDownAngleOld;
        float slopeSideAngle;
        float gravityScale;
        float currentSpeed;
        float yInput;
        float xInput;
        bool lookingRight = true;
        bool isOnSlope = false;
        public bool isGrounded;
        bool isJumping;
        bool canWalkOnSlope;
        bool isMovingOnLadder = false;
        bool isDetectingLadder = false;
        bool isMovingUp = false;
        bool isMovingOnWall = false;
        int counter = 0;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            gravityScale = rb.gravityScale;
            colliderSize = GetComponent<CapsuleCollider2D>().size;
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        // The ground will be checked all frame tu ensure the correct detection among all actions
        private void Update()
        {
            SlopeCheck(xInput);
            CheckGround();
            CheckForLadder();
        }

        public void GetMovement(float xInput, float yInput)
        {
            this.xInput = xInput;
            this.yInput = yInput;
        }

        #region SlopeMovement
        public void SlopeCheck(float xInput)
        {
            Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2);
            SlopeCheckVertical(checkPos, xInput);
            SlopeCheckHorizontal(checkPos);
        }
        void SlopeCheckHorizontal(Vector2 checkPos)
        {
            RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, whatIsSlope);
            RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, whatIsSlope);
            Debug.DrawRay(checkPos, transform.right * slopeCheckDistance, Color.blue);
            Debug.DrawRay(checkPos, -transform.right * slopeCheckDistance, Color.blue);

            if (slopeHitFront)
            {
                //print("Slope In Front");
                isOnSlope = true;
                slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
                //print(slopeSideAngle);
            }
            else if (slopeHitBack)
            {
                //print("SlopeInBack");
                isOnSlope = true;
                //slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
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
                //print(slopeNormalPerp);
                slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeDownAngle != slopeDownAngleOld)
                {
                    isOnSlope = true;
                    slopeDownAngleOld = slopeDownAngle;
                }
                Debug.DrawRay(hit.point, hit.normal, Color.green);
                Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            }
            //print(isOnSlope + "  " + canWalkOnSlope);
            //print(" Slope down Angle  " + slopeDownAngle);
            //print(slopeSideAngle);
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
                isJumping = false;
            }
        }



        #region LadderControl

        private void CheckForLadder()
        {
            counter++;
            if (counter >= 10)
            {
                if (!IsMovingOnLadder())
                {
                    LadderDetectionOutsideLadderMovement(0.5f);
                }
                else
                {
                    LadderDetectionInsideLadderMovement();
                }
                counter = 0;
            }
        }

        private void LadderDetectionOutsideLadderMovement(float minYInput)
        {
            if (yInput > minYInput)
            {
                //Debug.Log("CheckingLadder");
                isMovingUp = true;
                Debug.DrawRay(transform.position, Vector2.up, Color.gray);
                RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, -colliderSize.y / 2), Vector2.up, checkLadderDistance, whatIsLadder); ;
                if (hit)
                {
                    isDetectingLadder = true;
                    StartLadderMovementFromBelow(yInput);
                }
                else
                {
                    isDetectingLadder = false;
                }
            }

            if (yInput < -minYInput)
            {
                //Debug.Log("CheckingLadder");
                isMovingUp = false;
                Debug.DrawRay(transform.position, Vector2.down, Color.gray);
                RaycastHit2D hit = Physics2D.Raycast(ladderCheck.position, Vector2.down, checkLadderDistance, whatIsLadder);
                if (hit)
                {
                    isDetectingLadder = true;
                    StartLadderMovementFromAbove(yInput);
                }
                else
                {
                    isDetectingLadder = false;
                }
            }
        }


        private void LadderDetectionInsideLadderMovement()
        {
            if (yInput > 0)
            {
                //Debug.Log("Checking for Above Ladder Exit");
                Vector3 raycastOriginPosition = transform.position + new Vector3(0, -colliderSize.y / 2);
                isMovingUp = true;
                Debug.DrawRay(raycastOriginPosition, Vector2.up, Color.gray);
                RaycastHit2D hit = Physics2D.Raycast(raycastOriginPosition, Vector2.up, checkLadderDistance, whatIsLadder); ;
                if (hit)
                {
                    isDetectingLadder = true;
                }
                else
                {
                    ExitLadderFromAbove();
                    isDetectingLadder = false;
                }
            }
            if (yInput < 0 && isGrounded)
            {
                ExitLadderFromBelow();
            }
        }

        void StartLadderMovementFromBelow(float yInput)
        {
            Debug.Log("LadderMovement Started Below");
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, -1.5f), Vector2.up, checkLadderDistance, whatIsLadder);
            transform.position = new Vector3(hit.collider.transform.position.x, transform.position.y, 0);
            isMovingOnLadder = true;
            GravityScale(0);
        }
        void StartLadderMovementFromAbove(float yInput)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, checkLadderDistance, whatIsGround);
            if (!hit.collider.CompareTag("LadderTop")) return;
            Debug.Log("Started Climbing a ladder");
            hit.collider.transform.parent.GetComponent<Ladder>().InvertPlatform();
            rb.position = new Vector2(hit.collider.transform.position.x, hit.point.y+colliderSize.y/2);
            isMovingOnLadder = true;
            GravityScale(0);
        }

        void ExitLadderFromAbove()
        {
            Debug.Log("Exiting Ladder from Above");
            RaycastHit2D hit = Physics2D.Raycast(transform.position /*+ new Vector3(0, colliderSize.y)*/, Vector2.down, 2, whatIsGround);
            if (hit)
            {
                rb.MovePosition(hit.point + new Vector2(0,colliderSize.y / 2));
            }
            rb.velocity = new Vector2(0, 0);
            isMovingOnLadder = false;
            GravityScale(gravityScale);
        }

        void ExitLadderFromBelow()
        {
            //Debug.Log("Downside Ladder check");
            isMovingOnLadder = false;
            GravityScale(gravityScale);
        }

        #endregion

        //Action triggered in PlayerController. Depending on the player state is that it will do it or not. 
        //The decision is taken in the PlayerMovement script
        public void Jump()
        {
            //Debug.Log("Wants to Jump");
            if (IsMovingOnWall())
            {
                isMovingOnWall = false;
                canDoubleJump = true;
                JumpMovement();
                GravityScale(gravityScale);
                rb.drag = 1;
                return;
            }
            else if (isGrounded && canWalkOnSlope)
            {
                JumpMovement();
                return;
            }
            else if (isOnSlope && canDoubleJump)
            {
                JumpMovement();
                canDoubleJump = false;
                //Debug.Log("1 " + canDoubleJump);
                return;
            }
            else if (canDoubleJump && canWalkOnSlope)
            {
                JumpMovement();
                canDoubleJump = false;
                //Debug.Log("2 " + canDoubleJump);
                return;
            }
        }

        private void JumpMovement()
        {
            isJumping = true;
            rb.velocity = Vector2.up * jumpVelocity;
            animator.SetTrigger("Jump");
        }



        // Main Method of the Class
        public void PlayerMoving(float xInput, float yInput, bool isInteractingWithObject)
        {
            this.xInput = xInput;
            this.yInput = yInput;
            animator.SetFloat("yVelocity", rb.velocity.y);
            if (IsMovingOnWall())
            {
                WallMovement(xInput, yInput);
                return;
            }
            else if (IsMovingOnLadder()) 
            {
                LadderMovement(yInput, isInteractingWithObject);
                return;
            }
            else
            {
                FreeMovement(xInput, yInput, isInteractingWithObject);
            }
        }

        private void WallMovement(float xInput, float yInput)
        {
            rb.velocity = new Vector2(xInput, yInput) * maxWallSpeed;
        }

        void LadderMovement(float yInput, bool isInteractingWithObject)
        {
            rb.velocity = new Vector2(0, yInput * currentSpeed);
        }
        public void FreeMovement(float xInput, float yInput, bool isInteractingWithObject)
        {
            Flip(xInput, isMovingOnLadder, isInteractingWithObject);
            MovementOutsideLadder(xInput);
        }

        #region InLadderMovement

        public bool IsMovingOnLadder()
        {
            return isMovingOnLadder;
        }

        #endregion


        #region WallMovement
        public void SwitchWallMovement()
        {
            isMovingOnWall = !isMovingOnWall;
            CheckIsMovingOnWall(isMovingOnWall);
        }
        public void SetIsMovingOnWall(bool state)
        {
            isMovingOnWall = state;
            CheckIsMovingOnWall(isMovingOnWall);
        }
        private void CheckIsMovingOnWall(bool isMovingOnWall)
        {
            if (isMovingOnWall)
            {
                rb.gravityScale = 0;
                isMovingOnWall = true;
                rb.drag = 0;
            }
            else
            {
                rb.gravityScale = gravityScale;
                isMovingOnWall = false;
                rb.drag = 1;
            }
        }

        public bool IsMovingOnWall()
        {
            return isMovingOnWall;
        }
        #endregion


        #region Outside Ladder Movement
        void MovementOutsideLadder(float xInput)
        {
            float xVelocity;
            if (isGrounded && !isOnSlope && !isJumping)
            {
                xVelocity = xInput * currentSpeed;
                rb.velocity = new Vector2(xVelocity, rb.velocity.y);
                animator.SetFloat("xVelocity", Mathf.Abs(xVelocity));
                //print("Not In Slope");
            }

            if (isGrounded && isOnSlope && !isJumping && canWalkOnSlope)
            {
                xVelocity = -xInput * currentSpeed * slopeNormalPerp.x;
                rb.velocity = new Vector2(xVelocity, -xInput * currentSpeed * slopeNormalPerp.y);
                animator.SetFloat("xVelocity", Mathf.Abs(xVelocity));
                //print("In Slope");
            }
            if (!isGrounded)
            {
                rb.velocity = new Vector2(xInput * currentSpeed, rb.velocity.y);
                rb.sharedMaterial = noFriction;
                //print("In The Air");
            }
        }

        #endregion


        private void Flip(float xInput, bool isMovingInLadder, bool isInteractingWithObject)
        {
            if (isInteractingWithObject)
            {
                currentSpeed = maxInteractingSpeed;
                return;
            }
            if (isMovingInLadder) return;
            currentSpeed = maxRunningSpeed;
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

        //Might need to check the xInput here
        //Before was the movement, but that can be access in this script
        //Must be access through the Input Handler
        public void EvadeMovement()
        {
            //if (isEvading) return;
            gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
            //isEvading = true;
            if (xInput == 0)
            {
                Debug.Log("Simple jump behind evasion");
                animator.SetInteger("Evade", 1);
            }
            if (xInput != 0)
            {
                //Debug.Log("Will evade with a roll to the left");
                animator.SetInteger("Evade", 10);
            }

        }

        #region Events
        void CreateDust()
        {
            if (!isGrounded) return;
            dust.Play();
        }

        void GravityScale(float gravityScale)
        {
            rb.gravityScale = gravityScale;
        }

        void SimpleEvasionForceApplication()
        {
            //Debug.Log("Applying Evasion force");
            rb.velocity = new Vector2(simpleEvasionInitialVelocity.x * -transform.right.x, simpleEvasionInitialVelocity.y);
        }
        void RollEvasionForceApplication()
        {
            //Debug.Log("Applying Evasion force");
            coroutine = StartCoroutine(RollEvasionForce());
        }
        IEnumerator RollEvasionForce()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                //Debug.Log("Evading");
                PlayerMoving(rollSpeed.x * transform.right.x, rollSpeed.y, false);
            }
        }


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
                PlayerMoving(forwardAttackSpeed.x*transform.right.x, forwardAttackSpeed.y, false);
            }
        }

        public void SetVelocityTo0()
        {
            //Debug.Log("Player Stopped");
            if (coroutine != null) StopCoroutine(coroutine);
            rb.velocity = new Vector2(0, 0);
        }

        void JumpSound()
        {
            audioSource.PlayOneShot(jumpSound);
        }

        public PhysicsMaterial2D FullFriction()
        {
            return fullFriction;
        }

        #endregion
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(groundCheck1.position, groundCheckRadius);
            Gizmos.DrawWireSphere(groundCheck2.position, groundCheckRadius);
        }
    }

}
