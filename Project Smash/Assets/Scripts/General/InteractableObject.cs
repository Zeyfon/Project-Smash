using System.Collections;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    [Tooltip("The distance from the center of the object to the player to avoid any collision issues")]
    public float xOffset = 1f;
    [SerializeField] float raycastDistance = 5;
    [SerializeField] LayerMask whatIsBreakable;
    Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        //print(xOffset);
    }

    public void ApplyImpulseForce( Vector3 playerPosition, int damage)
    {
        float impulseForce = (float)damage * 600;
        Vector2 forceDirection = GetForceDirection(playerPosition);
        rb.AddForce(forceDirection * impulseForce, ForceMode2D.Impulse);
        StartCoroutine(CheckingForInteraction(forceDirection.x));
    }

    Vector2 GetForceDirection(Vector3 playerPosition)
    {
        float relativePlayerPosition = transform.position.x - playerPosition.x;
        if (relativePlayerPosition >= 0)
        {
            print("Impulsing Right");
            return new Vector2(1, 0);
        }
        else
        {
            print("Impulsing Left");
            return new Vector2(-1, 0);
        }
    }

    IEnumerator CheckingForInteraction(float xDirection)
    {
        while (Mathf.Abs(rb.velocity.x) > 0)
        {
            yield return new WaitForFixedUpdate();
            Vector2 localVelocity = transform.InverseTransformDirection(rb.velocity);
            if (localVelocity.x > 0)
            {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right*xDirection, raycastDistance, whatIsBreakable);
                Debug.DrawRay(transform.position, transform.right * raycastDistance);
                if (hit)
                {
                    print(hit.collider.gameObject.name + "  found");
                    if (Mathf.Abs(rb.velocity.x) > 5)
                    {
                        hit.collider.GetComponent<BoxCollider2D>().isTrigger = true;
                    }
                    yield break;
                }
            }
            else
            {
                //print("Moving Left");
            }
        }
    }
}
