using PSmash.Movement;
using PSmash.Combat;
using UnityEngine;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

namespace PSmash.Control
{
    public class PlayerController : MonoBehaviour,IIsAttacking
    {

        public static Transform playerTransform;

        Transform interactableObjectTransform;
        PlayerMovement playerMovement;
        PlayerInteractions playerInteractions;
        PlayerFighter fighter;
        SecondaryWeaponSystem secondaryWeapons;
        Animator animator;
        Rigidbody2D rb;
        bool isInteractingWithObject = false;
        bool interactableObjectSet = false;
        bool isInteractableObjectClose = false;
        bool isInteractingButtonPressed = false;
        bool isGrounded = true;
        bool isAttacking = false;
        bool isEvading = false;
        float gravityScale;
        float xInput;
        float yInput;

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
            secondaryWeapons = GetComponent<SecondaryWeaponSystem>();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            gravityScale = rb.gravityScale;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //if (InteractingWithObjects()) return;
            if (isAttacking || isEvading) return;
            InteractWithMovement();
        }

        private void InteractWithMovement()
        {
            playerMovement.PlayerMoving(xInput, yInput, isInteractingWithObject);
        }

        //Method called from the InputHandler
        public void GetMovement(float xInput, float yInput)
        {
            this.xInput = xInput;
            this.yInput = yInput;
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
            if (isAttacking || isEvading|| playerMovement.IsMovingOnLadder()) return;
            playerMovement.Jump();
        }

        public void AttackButtonPressed(bool isPressedAttack)
        {
            if (isEvading || playerMovement.IsMovingOnWall() || playerMovement.IsMovingOnLadder()) return;

            if (animator.GetInteger("Attack") == 0)
            {
                StartCoroutine(CheckAttacking());
                isAttacking = true;
            }
            fighter.Attack(isPressedAttack, isGrounded, yInput);
        }

        public void InteractButtonPressed(bool isInteractionButtonPressed)
        {
            isInteractingButtonPressed = isInteractionButtonPressed;
        }

        public void ParryButtonPressed()
        {
            Debug.Log("Parry button Pressed");
            if (isAttacking || isEvading || playerMovement.IsMovingOnWall() || playerMovement.IsMovingOnLadder()) return;
            Debug.Log("Will Parry");
            isAttacking = true;
            StartCoroutine(CheckAttacking());
            fighter.Parry();
        }

        public void EvadeButtonPressed()
        {
            if (isAttacking || isEvading || playerMovement.IsMovingOnWall() || playerMovement.IsMovingOnLadder()) return;
            playerMovement.EvadeMovement();
            isEvading = true;
            StartCoroutine(CheckEvasion());
        }

        public void SecondaryWeaponButtonPressed()
        {

            if (isAttacking || isEvading || playerMovement.IsMovingOnLadder()) return;
            Debug.Log("Secondary Weapon Action");
            secondaryWeapons.PerformSecondaryWeaponAction(this);
        }
        public void SecondaryWeaponSelectorPressed()
        {
            if (isAttacking) return;
            secondaryWeapons.ChangeActiveWeapon();
        }

        public void IIsAttacking()
        {
            Debug.Log("Interface Activated");
            isAttacking = true;
            StartCoroutine(CheckAttacking());
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

        public void SetIsAttacking(bool state)
        {
            isAttacking = state;
        }
    }
}
