using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;

namespace PSmash.Movement
{
    public class LadderMovementControl
    {
        public bool CheckForClimbingLadder(Transform oneWayPlatform, float yInput, bool isGrounded, bool isCollidingWithOneWayPlatform)
        {
            //Debug.Log("Looking for Ladder");
            if (isCollidingWithOneWayPlatform && yInput < -0.9f)
            {
                oneWayPlatform.GetComponent<OneWayPlatform>().RotatePlatform();
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
        /// <summary>
        /// Sets the player to climb the ladder.
        /// Used by the LadderClimbingState
        /// </summary>
        public void LadderMovementSetup(Transform transform, float ladderPositionX)
        {
            Rigidbody2D rb = transform.GetComponent<Rigidbody2D>();
            rb.position = new Vector3(ladderPositionX, transform.position.y, 0);
            rb.velocity = new Vector2(0, 0);
            rb.gravityScale = 0;
        }


        /// <summary>
        /// The controlled movement on the ladder
        /// Used by the LadderClimbingState
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxLadderMovementSpeed"></param>
        public void LadderMovement(Vector2 input, float maxLadderMovementSpeed, Transform transform, Vector2 colliderSize, LayerMask whatIsLadderTop, Animator animator, Rigidbody2D rb,   float gravityScale, LayerMask whatIsGround, bool isGrounded, bool isCollidingWithOneWayPlatform, float checkLadderDistance)
        {
            animator.SetFloat("climbingSpeed", Mathf.Abs(input.y));
            rb.velocity = new Vector2(0, input.y * maxLadderMovementSpeed);
        }

        /// <summary>
        /// The options for the player to exit the ladder without any additional input
        /// </summary>
        /// <param name="yInput"></param>
        public bool CheckToExitLadder(float yInput, Transform transform, Vector2 colliderSize, LayerMask whatIsLadderTop, Animator animator, Rigidbody2D rb, float gravityScale, LayerMask whatIsGround, bool isGrounded, bool isCollidingWithOneWayPlatform, float checkLadderDistance)
        {
            if (yInput > 0 && IsOneWayPlatformBeneathMe(transform, colliderSize, whatIsLadderTop))
            {        
                FinishClimbingFromAbove(animator, rb, gravityScale);
                RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, colliderSize.y / 2, whatIsLadderTop);
                rb.MovePosition(hit.point);
                //Debug.Break();
                return true;
            }

            else if (yInput < 0 && isGrounded && !isCollidingWithOneWayPlatform)
            {
                //print("Exiting Ladder from below");
                RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, colliderSize.y / 2), Vector2.down, checkLadderDistance, whatIsGround);
                if (!hit || hit.collider.CompareTag("ThinPlatform"))
                {
                    return false;
                }
                else
                {
                    FinishClimbingFromBelow(animator, rb, gravityScale);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns true if a One Way Platform is below the player 
        /// </summary>
        /// <returns></returns>
        bool IsOneWayPlatformBeneathMe(Transform transform, Vector2 colliderSize, LayerMask whatIsLadderTop)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, colliderSize.y / 2, whatIsLadderTop);
            Debug.DrawRay(transform.position, Vector2.down * colliderSize.y / 2,Color.red);
            if (hit && hit.collider.CompareTag("ThinPlatform"))
            {
                return true;
            }
            else 
                return false;
        }

        /// <summary>
        /// End Ladder Climbing going back to idle inmediately
        /// </summary>
        void FinishClimbingFromBelow(Animator animator, Rigidbody2D rb, float gravityScale)
        {
            rb.gravityScale = gravityScale;
            animator.SetInteger("LadderMovement", 10);
        }

        /// <summary>
        /// End Ladder Climbing using the Finish Climbing Ladder Animation
        /// </summary>
        void FinishClimbingFromAbove(Animator animator, Rigidbody2D rb, float gravityScale)
        {
            rb.velocity = new Vector2(0, 0);
            animator.SetInteger("LadderMovement", 5);
        }
    }


}
