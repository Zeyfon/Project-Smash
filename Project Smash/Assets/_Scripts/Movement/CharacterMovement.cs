using PSmash.Inventories;
using System.Collections;
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
        protected bool isMovementOverriden = false;

        protected bool isGrounded;
        protected float speedMovementModifier = 1;

        Vector2 impulseDirection;


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
                    //if (!gameObject.CompareTag("Player"))
                        //print(gameObject.name + "  " + rb.velocity);
                }
            }
        }

        /// <summary>
        /// Disables movement control by other objects.
        /// Apply the attack force to the entity.
        /// Waits till it passes the force to restore movement control.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="attackForce"></param>
        public virtual void ApplyAttackImpactReceived(Transform attacker, Weapon weapon, int layer1, int layer2)
        {
            StartCoroutine(ApplyKnockback_CR(attacker, weapon, layer1, layer2));
        }

        IEnumerator ApplyKnockback_CR(Transform attacker, Weapon weapon,int layer1, int layer2)
        {
            isMovementOverriden = true;
            gameObject.layer = layer1;
            yield return null;
            float tempDrag = rb.drag;
            PhysicsMaterial2D tempPhysicsMaterial = rb.sharedMaterial;
            Vector2 attackDirection = (transform.position - attacker.position).normalized;
            float timer = 0;
            //float speedFactor = attackForce / baseSpeed;
            //TODO
            //The time must be adjusted accordingly to the weapon with was damaged. 
            //float timerLimit = Mathf.Log10(attackForce)/ ((attackForce/2.4f)+1);
            //// 
            float speedFactor = weapon.GetAttackForce();
            float timerLimit = weapon.GetAttackForceTime();


            rb.sharedMaterial = lowFriction;
            rb.drag = 2;

            while (timer < timerLimit)
            {
                timer += Time.fixedDeltaTime;
                print(timer);
                impulseDirection = slope.GetMovementDirectionWithSlopecontrol(transform.position, attackDirection, slopeCheckDistance, whatIsGround);
                Movement(impulseDirection, isGrounded, speedFactor, speedMovementModifier);
                yield return new WaitForFixedUpdate();
            }
            rb.drag = tempDrag;
            rb.sharedMaterial = tempPhysicsMaterial;
            

            isMovementOverriden = false;

            if (gameObject.CompareTag("Player"))
            {
                timer = 0;
                while(timer < 0.5f)
                {
                    timer += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
                gameObject.layer = layer2;
            }
        }
    }
}

