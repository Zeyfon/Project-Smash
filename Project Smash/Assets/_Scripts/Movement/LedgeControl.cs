using UnityEngine;

public class LedgeControl
{
    Vector2 finalPosition;
    Vector2 ledge;
    float rayDistance = 0.7f;
    public bool IsLedgeFound(Transform transform,float yVelocity, LayerMask whatIsGround, Transform wallCheckUpper, Transform wallCheckMiddle)
    {
        RaycastHit2D isTouchingUpperWall;
        RaycastHit2D isTouchingMiddleWall;
        if (yVelocity > 0)
        {
            Debug.Log("Ledge check Upwards");
            isTouchingMiddleWall = GetIsTouchingMiddleWall(transform, wallCheckMiddle, whatIsGround);
            if (!isTouchingMiddleWall) return false;
            isTouchingUpperWall = GetIsTouchingUpperWall(transform, wallCheckUpper,whatIsGround);
            if (isTouchingUpperWall) return false;
        }
        else
        {
            //Debug.Log("Ledge Check Downwards");
            isTouchingUpperWall = GetIsTouchingUpperWall(transform, wallCheckUpper, whatIsGround);
            if (isTouchingUpperWall) return false;
            isTouchingMiddleWall = GetIsTouchingMiddleWall(transform, wallCheckMiddle, whatIsGround);
            if (!isTouchingMiddleWall) return false;
        }
        RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(transform.right.x, 2), Vector2.down, 2.5f, whatIsGround);
        Debug.Log(hit.collider.name);
        ledge = new Vector2(isTouchingMiddleWall.point.x, hit.point.y);       
        return true;
    }

    public void ClimbingLedge(Transform transform, Rigidbody2D rb, Animator animator, Vector2 colliderSize)
    {
        //Debug.Break();
        transform.gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
        rb.velocity = new Vector2(0, 0);
        animator.SetFloat("xVelocity", 0);
        animator.SetFloat("yVelocity", 0);
        rb.gravityScale = 0;
        Vector2 initialPosition = new Vector2(ledge.x + 0.1f * -transform.right.x, ledge.y - colliderSize.y);
        rb.MovePosition(initialPosition);
        animator.SetTrigger("ClimbLedge");
        finalPosition = new Vector2(ledge.x + 0.5f * transform.right.x, ledge.y + 0.00f);
    }


    public void FinishClimbingLedge(PlayMakerFSM pm, Transform transform, Rigidbody2D rb, float gravityScale)
    {
        //print("Finish Ledge Climb");
        transform.position = finalPosition;
        rb.gravityScale = gravityScale;
        transform.gameObject.layer = LayerMask.NameToLayer("Player");
        pm.SendEvent("ACTIONFINISHED");
    }

    RaycastHit2D GetIsTouchingUpperWall(Transform transform, Transform wallCheckUpper, LayerMask whatIsGround)
    {
        Debug.DrawRay(wallCheckUpper.position, transform.right * rayDistance, Color.blue);
        return Physics2D.Raycast(wallCheckUpper.position, transform.right, rayDistance, whatIsGround);
    }

    RaycastHit2D GetIsTouchingMiddleWall(Transform transform, Transform wallCheckMiddle, LayerMask whatIsGround)
    {
        Debug.DrawRay(wallCheckMiddle.position, transform.right * rayDistance, Color.blue);
        return Physics2D.Raycast(wallCheckMiddle.position, transform.right, rayDistance, whatIsGround);
    }



}
