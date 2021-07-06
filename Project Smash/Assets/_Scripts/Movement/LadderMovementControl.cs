using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;
using System;

namespace PSmash.Movement
{
    public class LadderMovementControl
    {
        public bool CheckForClimbingLadder(Transform oneWayPlatform, Transform playerTransform, Transform ladderTransform, float yInput, bool isGrounded, bool isCollidingWithOneWayPlatform)
        {
            //Debug.Log("Looking for Ladder");
            if (isCollidingWithOneWayPlatform && yInput < -0.8f)
            {
                Debug.Log("Entering Ladder with One Way Platform on top");
                //oneWayPlatform.GetComponent<OneWayPlatform>().RotatePlatform();
                LadderMovementSetup(playerTransform, ladderTransform);
                RunGoingDownAnimation(playerTransform);
                SetPlayerBeneathTheOneWayPlatform(oneWayPlatform, playerTransform);
                return true;
            }
            else if (!isGrounded && Mathf.Abs(yInput) > 0.9f)
            {
                Debug.Log("Is not Grounded");

                return true;
            }
      
            else if (isGrounded && !isCollidingWithOneWayPlatform && yInput > 0.8f)
            {
                Debug.Log("Is Grounded");
                return true;
            }
            else
                return false;
        }

        private void RunGoingDownAnimation( Transform playerTransform)
        {
            //TODO 
            //Here must be the animation running for the player to go beneath the OneWayPlatform. 
            //(Like the ClimgingEdge Animation with the displacement included)
            //playerTransform.GetComponent<Animator>().SetInteger("ladder",80);
        }

        private void SetPlayerBeneathTheOneWayPlatform(Transform oneWayPlatform, Transform playerTransform)
        {
            float newYPosition = playerTransform.position.y - (oneWayPlatform.GetComponent<BoxCollider2D>().size.y + playerTransform.GetComponent<CapsuleCollider2D>().size.y + 0.1f);
           // Debug.Log(newPlayerPosition);
            //playerTransform.GetComponent<Rigidbody2D>().MovePosition(newPlayerPosition);
            playerTransform.position = new Vector3(playerTransform.position.x, newYPosition);
            //Debug.Break();

        }

        /// <summary>
        /// Sets the player to climb the ladder.
        /// Used by the LadderClimbingState
        /// </summary>
        public void LadderMovementSetup(Transform transform, Transform ladderTransform)
        {
            Rigidbody2D rb = transform.GetComponent<Rigidbody2D>();
            transform.position = new Vector3(ladderTransform.position.x, transform.position.y, 0);
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 0;
            transform.rotation = SetPlayerOrientation(ladderTransform);
        }

        private Quaternion SetPlayerOrientation(Transform ladderTransform)
        {
            bool isLadderLookingRight = ladderTransform.GetComponent<Ladder>().GetLadderOrientation();
            if (isLadderLookingRight)
            {
                return Quaternion.Euler(0, 0, 0);
            }
            else
            {
                return Quaternion.Euler(0, 180, 0);
            }
        }


        /// <summary>
        /// The controlled movement on the ladder
        /// Used by the LadderClimbingState
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxLadderMovementSpeed"></param>
        public void LadderMovement(Vector2 input, float maxLadderMovementSpeed, Animator animator, Rigidbody2D rb)
        {
            animator.SetFloat("climbingSpeed", input.y);
            rb.velocity = new Vector2(0, input.y * maxLadderMovementSpeed);
        }

        /// <summary>
        /// The options for the player to exit the ladder without any additional input
        /// </summary>
        /// <param name="yInput"></param>
        public bool CheckToExitLadder(float yInput, Transform transform, Vector2 colliderSize, Animator animator, Rigidbody2D rb, float gravityScale, LayerMask whatIsGround, bool isGrounded, float checkLadderDistance)
        {
            if (yInput < 0 && isGrounded)
            {
                //print("Exiting Ladder from below");
                RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, colliderSize.y / 2), Vector2.down, checkLadderDistance, whatIsGround);
                if (!hit || hit.collider.CompareTag("ThinPlatform"))
                {
                    return false;
                }
                else
                {
                    rb.gravityScale = gravityScale;
                    animator.SetInteger("LadderMovement", 10);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }
    }


}
