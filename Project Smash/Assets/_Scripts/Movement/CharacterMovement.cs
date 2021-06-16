using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Movement
{
    public class CharacterMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] protected float baseSpeed;
        [SerializeField] protected float slopeCheckDistance = 0.5f;
        [SerializeField] protected LayerMask whatIsGround;

        [Header("PhysicsMaterials")]
        [SerializeField] protected PhysicsMaterial2D noFriction = null;
        [SerializeField] protected PhysicsMaterial2D lowFriction = null;
        [SerializeField] protected PhysicsMaterial2D fullFriction = null;

        protected SlopeControl slope = new SlopeControl();
        protected Rigidbody2D rb;


        /// <summary>
        /// 
        /// </summary>
        /// <param name="movementDirection"></param> Must be normalized
        /// <param name="isGrounded"></param>
        /// <param name="speedFactor"></param>
        /// <param name="speedModifier"></param>
        public virtual void Movement(Vector2 movementDirection, bool isGrounded,float speedFactor, float speedModifier)
        {
            float speedModified = baseSpeed * speedFactor * speedModifier;
            float xVelocity;
            if (!isGrounded)
            {
                rb.sharedMaterial = noFriction;
                xVelocity = movementDirection.x * speedModified;
                rb.velocity = new Vector2(xVelocity, rb.velocity.y);
            }
            else if (movementDirection.magnitude == 0)
            {
                rb.sharedMaterial = lowFriction;
                rb.velocity = new Vector2(0, 0);
            }
            else
            {
                rb.sharedMaterial = noFriction;
                Vector2 directionAfterSlope = slope.GetMovementDirectionWithSlopecontrol(transform.position, movementDirection, slopeCheckDistance, whatIsGround);
                if (directionAfterSlope.sqrMagnitude == 0)
                {
                    //Entering this part means that the slope raycast does not detect any ground and the isGround bool is still true
                    xVelocity = movementDirection.x * speedModified;// * Mathf.Abs(movementDirectionNormalized.x);
                    rb.velocity = new Vector2(xVelocity, rb.velocity.y);
                }
                else
                {
                    xVelocity = directionAfterSlope.x * speedModified;
                    float yVelocity = directionAfterSlope.y * speedModified;
                    rb.velocity = new Vector2(xVelocity, yVelocity);
                    if (!gameObject.CompareTag("Player"))
                        print(gameObject.name + "  " + rb.velocity);
                }
            }
        }
    }
}

