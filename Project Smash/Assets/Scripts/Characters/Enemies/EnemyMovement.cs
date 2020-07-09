using PSmash.Control;
using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float speed = 5;

    [Header("SlopeControl")]
    [SerializeField] float slopeCheckDistance = 0.5f;
    [SerializeField] float maxSlopeAngle = 45;
    [SerializeField] PhysicsMaterial2D fullFriction = null;
    [SerializeField] PhysicsMaterial2D lowFriction = null;
    [SerializeField] LayerMask whatIsSlope;
    [SerializeField] Transform groundCheck = null;
    [SerializeField] float groundCheckRadius = 0.5f;
    [SerializeField] LayerMask whatIsGround;

    [SerializeField] bool canDebug = false;

    Coroutine coroutine;
    Transform playerTransform;
    Rigidbody2D rb;
    Vector2 slopeNormalPerp;
    Vector2 colliderSize;
    float slopeDownAngleOld;
    float slopeDownAngle;
    float slopeSideAngle;
    float currentYAngle = 0;
    bool isOnSlope = false;
    bool canWalkOnSlope = true;
    bool isPlayerReachable = true;
    bool isMovingToTarget = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerTransform = PlayerController.playerTransform;
        colliderSize = GetComponent<CapsuleCollider2D>().size;
    }

    public void MoveTo(Vector3 destination, float speedfactor)
    {
       // Debug.Log("Want to move");
        CheckFlip();
        SlopeCheck();
        if(!IsGrounded())
        {
            Vector2 velocity = -slopeNormalPerp * speed * transform.right.x;
            velocity = new Vector2(velocity.x, 0);
        }
        else
        {
            rb.velocity = -slopeNormalPerp * speed * transform.right.x;
        }
    }

    bool IsGrounded()
    {
        bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, whatIsGround);
        return isGrounded;
    }
    void SlopeCheck()
    {
        Vector2 checkPos = transform.position - new Vector3(0.0f, colliderSize.y / 2f);
        //print(checkPos);
        SlopeCheckVertical(checkPos);
        SlopeCheckHorizontal(checkPos);
        //yield return null;
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
    void SlopeCheckVertical(Vector2 checkPos)
    {
        RaycastHit2D hit = Physics2D.Raycast(checkPos + new Vector2(0,0.3f), Vector2.down, slopeCheckDistance, whatIsSlope);
        if (canDebug)
        {
           if(hit) Debug.Log(hit);
            else
            {
                Debug.LogWarning(hit + "did not found anithing");
            }
        
        }
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
            //Debug.Break();
        }
        //print(canWalkOnSlope + "  " + isOnSlope);
        //print(slopeDownAngle+ "  " + slopeSideAngle);
        if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
        {
            //print(slopeDownAngle + "  " + slopeSideAngle);
            canWalkOnSlope = false;
        }
        else
        {
            canWalkOnSlope = true;
        }

        if (!canWalkOnSlope && isOnSlope)
        {
            isPlayerReachable = false;
            //Debug.Log("Player cannot be reached");
        }
        else
        {
            isPlayerReachable = true;
        }
        //print(canWalkOnSlope +" " + isOnSlope);

        //if (isOnSlope && !isPlayerNear && canWalkOnSlope)
        //{
        //    rb.sharedMaterial = fullFriction;
        //}
        //else
        //{
        //    rb.sharedMaterial = lowFriction;
        //}
    }

    public void CheckFlip()
    {
        Vector2 toTarget = (playerTransform.position - transform.position).normalized;
        //print(toTarget + "  " + transform.right + "  " + Vector2.Dot(toTarget, transform.right));
        if (Vector2.Dot(toTarget, transform.right) > 0)
        {
            //Do Nothing
            return;
        }
        else
        {
            Flip();
        }
    }

    private void Flip()
    {
        currentYAngle += 180;
        if (currentYAngle == 360) currentYAngle = 0;
        Quaternion currentRotation = new Quaternion(0, 0, 0, 0);
        Vector3 rotation = new Vector3(0, currentYAngle, 0);
        currentRotation.eulerAngles = rotation;
        transform.rotation = currentRotation;
    }

    public bool IsPlayerReachable()
    {
        return isPlayerReachable;
    }
    public void Cancel()
    {
        //StopCoroutine(coroutine);
        isMovingToTarget = false;
        //Debug.Log("Movement was canceled");
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

}
