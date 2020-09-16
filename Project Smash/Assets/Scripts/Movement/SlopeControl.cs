using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Movement
{
    public class SlopeControl : MonoBehaviour
    {
        float slopeCheckDistance;
        bool isOnSlope;
        float slopeSideAngle;
        float maxSlopeAngle;
        LayerMask whatIsSlope;
        Vector2 slopeNormalPerp;
        float slopeDownAngle;
        float slopeDownAngleOld;
        bool canWalkOnSlope;
        bool isPlayerReachable;

        EnemyMovement movement;

        private void Awake()
        {
            movement = GetComponent<EnemyMovement>();
        }
        public Vector2 SlopeCheck()
        {
            Vector2 checkPos = transform.position;
            SlopeCheckVertical(checkPos);
            SlopeCheckHorizontal(checkPos);
            return slopeNormalPerp;
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
            RaycastHit2D hit = Physics2D.Raycast(checkPos + new Vector2(0, 0.3f), Vector2.down, slopeCheckDistance, whatIsSlope);
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
    }
}


