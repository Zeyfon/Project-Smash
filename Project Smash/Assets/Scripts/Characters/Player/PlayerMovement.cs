using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
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
        [SerializeField] Transform ladderCheck = null;
        

        [Header("Movement")]
        [SerializeField] float maxRunningSpeed = 5;
        [SerializeField] float maxInteractingSpeed = 2;
        [SerializeField] float jumpForce = 4000;

        [Header("Attacks")]
        //[SerializeField] Vector2 splashAttackSpeed = new Vector2(0,0);


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

        [SerializeField] Vector2 forwardAttackSpeed = new Vector2(0,0);
        [SerializeField] AudioClip jumpSound;

        [SerializeField] ParticleSystem dust;

        Rigidbody2D rb;
        Animator animator;
        _Controller _controller;
        AudioSource audioSource;
        Coroutine coroutine;
        Vector2 slopeNormalPerp;
        Vector2 colliderSize;
        Vector2 movement;
        float slopeDownAngle;
        float slopeDownAngleOld;
        float slopeSideAngle;
        float gravityScale;
        float currentSpeed;
        float yInput;
        bool lookingRight = true;
        bool isOnSlope = false;
        public bool isGrounded;
        bool canJump = true;
        bool isJumping;
        bool canWalkOnSlope;
        bool isMovingInLadder = false;
        bool isDetectingLadder = false;
        bool isMovingUp = false;
        bool isEvading = false;

        private void Awake()
        {
            _controller = new _Controller();
        }
        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            gravityScale = rb.gravityScale;
            colliderSize = GetComponent<CapsuleCollider2D>().size;
            animator = GetComponent<Animator>();
            StartCoroutine(LookingForLadder());
            audioSource = GetComponent<AudioSource>();

        }
        //Called from PlayerController

        private void OnEnable()
        {
            _controller.Player.Enable();
            _controller.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
        }
        private void OnDisable()
        {
            _controller.Player.Disable();
            _controller.Player.Move.performed -= ctx => movement = ctx.ReadValue<Vector2>();
        }

        // The ground will be checked all frame tu ensure the correct detection among all actions
        private void Update()
        {
            CheckGround();
        }

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
                canJump = true;
                isJumping = false;
            }
        }

        //Action triggered in PlayerController. Depending on the player state is that it will do it or not. 
        //The decision is taken in the PlayerMovement script
        public void Jump()
        {
            //Debug.Log("Grounded  " + isGrounded + " CanJump  " + canJump);
            if ((!isGrounded || !canJump)) return;
            CreateDust();
            canJump = false;
            isJumping = true;
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            animator.SetTrigger("Jump");
        }



        // Main Method of the Class
        public void PlayerMoving(float xInput,float yInput, bool isInteractingWithObject)
        {
            //if (isEvading) return;
            this.yInput = yInput;
            SlopeCheck(xInput);
            Move(xInput, yInput, isInteractingWithObject);
            animator.SetFloat("yVelocity", rb.velocity.y);
        }

        public void Move(float xInput, float yInput, bool isInteractingWithObject)
        {
            Flip(xInput, isMovingInLadder, isInteractingWithObject);
            if (MovementInLadder(yInput, isInteractingWithObject)) return;
            MovementOutsideLadder(xInput);
        }
        bool MovementInLadder(float yInput, bool isInteractingWithObject)
        {
            if (isDetectingLadder && !isMovingInLadder && yInput > 0.8f)
            {
                ClimbingLadderStart(yInput);
                return true;
            }
            if(isMovingInLadder && !isDetectingLadder && isMovingUp)
            {
                UpperSideExitLadder();
                return true;
            }
            if (isMovingInLadder && isGrounded && !isMovingUp)
            {
                LowerSideExitLadder();

                return true;
            }
            if (isMovingInLadder)
            {
                rb.velocity = new Vector2(0, yInput * currentSpeed);
                return true;
            }
            return false;
        }

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

        void CreateDust()
        {
            if (!isGrounded) return;
            dust.Play();
        }
        IEnumerator LookingForLadder()
        {
            int counter = 0;
            while (true)
            {
                if (counter >= 5)
                {
                    if (yInput > 0.02f)
                    {
                        isMovingUp = true;
                        //Debug.DrawRay(transform.position, Vector2.up);
                        RaycastHit2D hit = Physics2D.Raycast(ladderCheck.position, Vector2.up, checkLadderDistance, whatIsLadder);
                        if (hit)
                        {
                            isDetectingLadder = true;
                        }
                        else
                        {
                            isDetectingLadder = false;
                        }
                    }

                    if (yInput < -0.02f)
                    {

                        isMovingUp = false;
                        // Debug.DrawRay(transform.position, Vector2.down);
                        RaycastHit2D hit = Physics2D.Raycast(ladderCheck.position, Vector2.down, checkLadderDistance, whatIsLadder);
                        if (hit)
                        {
                            isDetectingLadder = true;
                        }
                        else
                        {
                            isDetectingLadder = false;
                        }
                    }
                }
                counter++;
                yield return new WaitForEndOfFrame();
            }

        }
        void ClimbingLadderStart(float yInput)
        {
            Debug.Log("Started Climbing a ladder");
            RaycastHit2D hit = Physics2D.Raycast(ladderCheck.position, Vector2.up, checkLadderDistance, whatIsLadder);
            transform.position = new Vector3(hit.collider.transform.position.x, transform.position.y, 0);
            isMovingInLadder = true;
            ChangeGravityScale(0);
            rb.velocity = new Vector2(0, yInput * currentSpeed);
            //print("Moving in Ladder");
        }
        void UpperSideExitLadder()
        {
            //The next upadte must have a script that move the complete gameobject according to the LeaveStairs Upperside.
            //This will fix the problem when is about to exit.
            //That way the player will not be controlling the gameobject when exiting.
            rb.MovePosition(transform.position + new Vector3(0, 0.2f));
            Debug.Log("Upside Ladder check");
            print("Ladder Moving " + isMovingInLadder + " IsInLadder  " + isDetectingLadder + " isMovingUp " + isMovingUp);
            isMovingInLadder = false;
            ChangeGravityScale(gravityScale);
        }

        void LowerSideExitLadder()
        {
            //The next upadte must have a script that move the complete gameobject according to the LeaveStairs Downside.
            //This will fix the problem when is about to exit.
            //That way the player will not be controlling the gameobject when exiting.
            Debug.Log("Downside Ladder check");
            //print("Ladder Moving " + isMovingInLadder + " isGrounded  " + isGrounded + " IsInLadder  " + isDetectingLadder);
            isMovingInLadder = false;
            ChangeGravityScale(gravityScale);
            //Debug.Break();
        }

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

        void ChangeGravityScale(float gravityScale)
        {
            rb.gravityScale = gravityScale;
        }

        public void EvadeMovement()
        {
            //if (isEvading) return;
            gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
            //isEvading = true;
            if (movement.x == 0)
            {
                Debug.Log("Simple jump behind evasion");
                animator.SetInteger("Evade", 1);
            }
            if (movement.x != 0)
            {
                //Debug.Log("Will evade with a roll to the left");
                animator.SetInteger("Evade", 10);
            }

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
        public void GravityAdjust()
        {

            rb.gravityScale = 0;
            rb.sharedMaterial = fullFriction;
            rb.velocity = new Vector2(0, 0);
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
                //rb.velocity = new Vector2(forwardAttackSpeed.x * transform.right.x, forwardAttackSpeed.y);
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
            //audioSource.clip = jumpSound;
            //audioSource.pitch = Unity.Mathematics.Random[]
            audioSource.PlayOneShot(jumpSound);
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(groundCheck1.position, groundCheckRadius);
            Gizmos.DrawWireSphere(groundCheck2.position, groundCheckRadius);
        }
    }

}
