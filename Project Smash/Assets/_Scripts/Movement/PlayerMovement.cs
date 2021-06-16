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
    public class PlayerMovement : CharacterMovement, ISaveable
    {

        #region Inspector
        //[SpineBone(dataField: "skeletonRenderer")]
        //[SerializeField] public string boneName;

        [Header("Movement")]
        [SerializeField] AudioSource footStepAudioSource = null;

        [Header("GroundCheck")]
        [SerializeField] float groundCheckRadius = 0.1f;
        [SerializeField] Vector2 groundCheckPosition;

        [Header("SlopeManagement")]
        [SerializeField] float maxSlopeAngle = 40;

        [Header("Jump")]
        [SerializeField] AudioClip jumpSound = null;
        [SerializeField] bool canDoubleJump = true;
        [SerializeField] AudioClip landingSound = null;

        [Header("LadderMovement")]
        [SerializeField] float climbingLadderSpeedFactor = 0.4f;
        [SerializeField] float checkLadderDistance = 1;
        [SerializeField] LayerMask whatIsLadder;
        [SerializeField] LayerMask whatIsLadderTop;
        [SerializeField] Transform ladderCheck = null;
        [SerializeField] AudioClip climbingLadderSound = null;
        [SerializeField] AudioClip climbingWallSound = null;

        [Header("LedgeControl")]
        [SerializeField] Transform wallCheckUpper = null;
        [SerializeField] Transform wallCheckMiddle = null;
        [SerializeField] AudioSource climbingLegeAudioSource = null;

        [Header("Evasion")]
        [SerializeField] AudioClip backJumpSound = null;
        [SerializeField] AudioClip rollsound = null;

        [Header("Extra")]
        [SerializeField] ParticleSystem dust = null;

        #endregion

        public delegate void PlayerOnWall(bool state);
        public event PlayerOnWall OnPlayerWallState;

        FsmObject currentFSM;
        LedgeControl ledgeControl = new LedgeControl();
        LadderMovementControl ladderControl = new LadderMovementControl();
        Transform oneWayPlatform;
        Animator animator;
        AudioSource audioSource;
        Vector2 colliderSize;
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
        bool isPlayerDetectingLadder = false;

        // Start is called before the first frame update
        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            colliderSize = GetComponent<CapsuleCollider2D>().size;
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
        }

        void Start()
        {
            if (rb == null) Debug.LogWarning("Rigidbody was not found");
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
            float xVelocity = (transform.right.x * rb.velocity.x);
            float yVelocity = rb.velocity.y;
            animator.SetFloat("xVelocity", xVelocity);
            animator.SetFloat("yVelocity", yVelocity);

        }
        ////////////////////////////////////////////////////////////////////////////PUBLIC ///////////////////////////////////////////////////////////////////////////////////////

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
                Flip(input.x);
            }
            Move(input, speedFactor);
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

        public bool IsLedgeFound()
        {
            return ledgeControl.IsLedgeFound(transform, rb.velocity.y, whatIsGround, wallCheckUpper, wallCheckMiddle);
        }

        /// <summary>
        /// Used by the Movement FSM to initialize the Climbing Edge State
        /// </summary>
        /// <param name="ledge"></param>
        public void ClimbingLedge()
        {
            ledgeControl.ClimbingLedge(transform, rb, animator, GetComponent<CapsuleCollider2D>().size);
        }

        /// <summary>
        /// Check both if a ladder is detected and player input to start climbing the ladder.
        /// Used by the Movement State
        /// </summary>
        /// <param name="input"></param>
        public bool CheckForLadder(Vector2 input)
        {
            if (isPlayerDetectingLadder)
            {
                if (ladderControl.CheckForClimbingLadder(oneWayPlatform, input.y, isGrounded, isCollidingWithOneWayPlatform))
                {
                    print("Entering Ladder Movement State");
                    ladderControl.LadderMovementSetup(transform, ladderPositionX);
                    canDoubleJump = true;
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }

        /// <summary>
        /// The controlled movement on the ladder
        /// Used by the LadderClimbingState
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxLadderMovementSpeed"></param>
        public void LadderMovement(Vector2 input, float maxLadderMovementSpeed)
        {
            ladderControl.LadderMovement(input, maxLadderMovementSpeed, transform, colliderSize, whatIsLadderTop, animator, rb, gravityScale, whatIsGround, isGrounded, isCollidingWithOneWayPlatform, checkLadderDistance);
        }

        public bool IsMovingOutsideOfLadder(Vector2 input)
        {
            return ladderControl.CheckToExitLadder(input.y, transform, colliderSize, whatIsLadderTop, animator, rb, gravityScale, whatIsGround, isGrounded, isCollidingWithOneWayPlatform, checkLadderDistance);
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

        /// <summary>
        /// Sets the impulse the player will do when performing an attack
        /// </summary>
        /// <param name="attackimpulse"></param>
        public void MovementImpulse(float attackimpulse)
        {
            Vector2 direction = slope.GetMovementDirectionWithSlopecontrol(transform.position, transform.right, slopeCheckDistance, whatIsGround);
            rb.velocity = new Vector2(direction.x , direction.y) * attackimpulse;
        }

        public void UpdatePlayerStoredPosition()
        {
            print("Initializing Player StoredPosition");
            storedPosition = transform.position;
        }

        //////////////////////////////////////////////////////////////////////////////////////PRIVATE/////////////////////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Set the isGround, isJumping and canDoubleJump depending on GroundDetection
        /// </summary>
        void GroundCheck()
        {
            isGrounded = Physics2D.OverlapCircle((Vector2)transform.position + groundCheckPosition, groundCheckRadius, whatIsGround);
            animator.SetBool("Grounded", isGrounded);
            canWalkOnSlope = slope.CanWalkOnThisGround(transform.position, transform.right, slopeCheckDistance, maxSlopeAngle, whatIsGround);
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

        void Move(Vector2 input, float speedFactor)
        {
            if (!slope.CanWalkOnThisGround(transform.position, transform.right, slopeCheckDistance, maxSlopeAngle, whatIsGround))
            {
                rb.sharedMaterial = noFriction;
                //You do not have control over your player is you cannot walk on slope
            }
            else
            {
                Movement(input, isGrounded, speedFactor,1);
            }
        } 

        //AnimEvent
        void FinishClimbingLedge()
        {
            ledgeControl.FinishClimbingLedge(currentFSM.Value as PlayMakerFSM, transform, rb, gravityScale);
        }

        //////////////////SOUNDS AND EFFECTS///////////////////
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
        /// Anim Event.
        void JumpSound()
        {
            audioSource.PlayOneShot(jumpSound);
        }

        //AnimEvent
        void ClimbingLedgeSound()
        {
            climbingLegeAudioSource.Play();
        }

        //AnimEvent
        void ClimbingLadderSound()
        {
            audioSource.PlayOneShot(climbingLadderSound);
        }

        /////////////////TRIGGERS AND COLLISION DETECTION////////////////

        void OnCollisionEnter2D(Collision2D collision)
        {
            //print("Collision Enter");
            if (collision.collider.CompareTag("ThinPlatform"))
            {
                isCollidingWithOneWayPlatform = true;
                oneWayPlatform = collision.collider.transform;
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
                isPlayerDetectingLadder = true;
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
                isPlayerDetectingLadder = false;
                ladderPositionX = 0;
            }

            if (collision.CompareTag("VirtualCamera"))
            {
                collision.GetComponent<IChangeVCamera>().ExtiCamArea();
            }
        }



        void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position + (Vector3)groundCheckPosition, groundCheckRadius);
        }



        ////////////////////////CAPTURE AND RESTORES STATES////////////////////
        public object CaptureState()
        {
            Tent checkpoint = FindObjectOfType<Tent>();
            if ((checkpoint != null && checkpoint.IsPlayerInSavePoint()) || Mathf.Approximately(storedPosition.magnitude, 0))
            {
                SerializableVector3 position = new SerializableVector3(transform.position);
                storedPosition = transform.position;
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

            if (isLoadLastScene && !Mathf.Approximately(storedPosition.magnitude,0))
            {
                transform.position = storedPosition;
            }
        }
    }

}
