using PSmash.Movement;
using PSmash.Combat;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace PSmash.Control
{
    public class PlayerController : MonoBehaviour
    {

        public static Transform playerTransform;
        public float xInput;
        public float yInput;
        public bool attack = false;
        public bool jump = false;

        Transform interactableObjectTransform = null;
        PlayerMovement playerMovement = null;
        PlayerInteractions playerInteractions = null;
        PlayerFighter fighter = null;
        Animator animator = null;
        Rigidbody2D rb;
        bool isInteractingWithObject = false;
        bool interactableObjectSet = false;
        bool isInteractableObjectClose = false;
        bool canJump = true;
        bool isJumping = false;
        bool isInteractingButtonPressed = false;
        bool isGrounded = true;
        bool isAttacking = false;
        bool canAttack = true;
        bool isEvading = false;
        float gravityScale;

        private void Awake()
        {
            if (playerTransform) Destroy(gameObject);
            playerTransform = transform;

        }
        // Start is called before the first frame update
        void Start()
        {
            playerInteractions = GetComponent<PlayerInteractions>();
            playerMovement = GetComponent<PlayerMovement>();
            fighter = GetComponent<PlayerFighter>();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            gravityScale = rb.gravityScale;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (isAttacking || isEvading) return;
            if (InteractingWithObjects())return;
            //Debug.Log("Can Move");
            InteractWithMovement();
        }

        private void InteractWithMovement()
        {
            playerMovement.PlayerMoving(xInput, yInput, isInteractingWithObject);
        }

        private bool InteractingWithObjects()
        {
            if (isInteractableObjectClose)
            {
                if (isInteractingButtonPressed && !isInteractingWithObject)
                {
                    print("Wants To Interact with Object");
                    //This Method must be called only once since will start a Coroutine in PlayerInteraction Script
                    playerInteractions.MovingObject(interactableObjectTransform);
                    isInteractingWithObject = true;
                    //canJump = false;
                }


                else if (!isInteractingButtonPressed && isInteractingWithObject)
                {
                    playerInteractions.StopInteracting(interactableObjectTransform);
                    isInteractingWithObject = false;
                    //canJump = true;
                }

                if (isInteractingWithObject)
                {
                    playerMovement.PlayerMoving(xInput, yInput, isInteractingWithObject);
                    return true;
                }
            }
            return false;
        }

        #region Update Main Functions

        // Check for Ladders with a Raycast with a Vector.up direction from from the feet.

        #endregion

        #region InteractableObject Trigger Detections
        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("InteractableObject"))
            {
                if (!interactableObjectSet)
                {
                    //print("InteractableObject In Area");
                    isInteractableObjectClose = true;
                    interactableObjectTransform = collision.transform;
                }
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("InteractableObject"))
            {
                //print("InteractableObject outside area");
                isInteractableObjectClose = false;
                interactableObjectTransform = null;
            }
        }
        #endregion
        #region ActionButtons
        //Action Buttons
        public void JumpButtonPressed()
        {
            //Debug.Log("Jump Button Pressed");
            //if (!isJumping || isAttacking) return;
            //Debug.Log("Will Jump");
            playerMovement.Jump();
            isJumping = true;
        }

        public void AttackButtonPressed(bool isPressedAttack)
        {
            isAttacking = true;
            StartCoroutine(CheckAttacking());
            fighter.Attack(isPressedAttack, isGrounded);
        }

        public void InteractButtonPressed(bool isInteractionButtonPressed)
        {
            isInteractingButtonPressed = isInteractionButtonPressed;
        }

        public void ParryButtonPressed()
        {
            Debug.Log("Parry button Pressed");
            if (isAttacking) return;
            Debug.Log("Will Parry");
            isAttacking = true;
            StartCoroutine(CheckAttacking());
            fighter.Parry();
        }

        public void EvadeButtonPressed()
        {
            if (isAttacking || isEvading) return;
            playerMovement.EvadeMovement();
            isEvading = true;
            StartCoroutine(CheckEvasion());
        }

        public void SubAttackButtonPressed()
        {
            if (isAttacking) return;
            isAttacking = true;
            StartCoroutine(CheckAttacking());
            fighter.SubAttack();
        }

        public void MolotoveBombButtonPressed()
        {
            if (isAttacking) return;
            isAttacking = true;
            StartCoroutine(CheckAttacking());
            fighter.MolotovAttack();
        }
        #endregion
        IEnumerator CheckAttacking()
        {
            while (animator.GetInteger("Attack") == 0)
            {
                //print("Waiting for Attack to Start");
                yield return new WaitForEndOfFrame();
            }
            while (animator.GetInteger("Attack") != 100)
            {
                //print("Waiting for Attack to End");
                yield return new WaitForEndOfFrame();
            }

            animator.SetInteger("Attack", 0);
            //print("Attack has ended");
            isAttacking = false;
            //yield return new WaitForSeconds(0.1f);
            //Debug.Log("Gravity 1");
            rb.gravityScale = gravityScale;
        }

        IEnumerator CheckEvasion()
        {
            while (animator.GetInteger("Evade") != 100)
            {
                yield return new WaitForEndOfFrame();
            }
            animator.SetInteger("Evade",0);
            //Debug.Log("Evasion Finished");
            gameObject.layer = LayerMask.NameToLayer("Player");
            isEvading = false;
        }

        //private void OnDrawGizmos()
        //{
        //    Gizmos.DrawWireSphere(groundCheck1.position, groundCheckRadius);
        //    Gizmos.DrawWireSphere(groundCheck2.position, groundCheckRadius);
        //}
    }
}
