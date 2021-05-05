using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Movement
{
    /// <summary>
    /// With several raycasts the entity will determine if it is over a slope and if it can move based on the maxAngle allowed
    /// </summary>
    public class SlopeControl 
    {
        float slopeSideAngle;
        float slopeDownAngleOld;
        /// <summary>
        /// Gets the data based on if the entity is over a slope or not
        /// </summary>
        /// <param name="checkPos"></param>
        /// <param name="transformRight"></param>
        /// <param name="slopeCheckDistance"></param>
        /// <param name="whatIsGround"></param>
        /// <returns></returns>
        public bool IsOnSlope(Vector2 checkPos, Vector2 transformRight, float slopeCheckDistance, LayerMask whatIsGround)
        {
            bool isOnSlope;
            RaycastHit2D slopeHitFront = Physics2D.Raycast(checkPos, transformRight, slopeCheckDistance, whatIsGround);
            RaycastHit2D slopeHitBack = Physics2D.Raycast(checkPos, -transformRight, slopeCheckDistance, whatIsGround);
            Debug.DrawRay(checkPos, transformRight * slopeCheckDistance, Color.blue);
            Debug.DrawRay(checkPos, -transformRight * slopeCheckDistance, Color.blue);

            ///Check if there is a slope contact with the frontal Raycast
            if (slopeHitFront && !slopeHitFront.collider.CompareTag("LadderTop") && Mathf.Abs(slopeHitFront.normal.x) < 0.9f)
            {
                isOnSlope = true;
                slopeSideAngle = Vector2.Angle(slopeHitFront.normal, Vector2.up);
                //Debug.Log(slopeHitFront.collider.gameObject);
                //Debug.Log("frontal angle  " + slopeSideAngle);
            }


            ///Checks if there is a slope with the back Raycast
            else if (slopeHitBack && !slopeHitBack.collider.CompareTag("LadderTop") && Mathf.Abs(slopeHitBack.normal.x) < 0.9f)
            {
                isOnSlope = true;
                slopeSideAngle = 0.0f;
                //slopeSideAngle = Vector2.Angle(slopeHitBack.normal, Vector2.up);
                //Debug.Log(slopeHitBack.collider.gameObject);
                //Debug.Log("back angle  " + slopeSideAngle);

            }
            else
            {
                isOnSlope = false;
                slopeSideAngle = 0.0f;
            }


            if (isOnSlope)
                return isOnSlope;
            else
            {
                RaycastHit2D hit = Physics2D.Raycast(checkPos + new Vector2(0, .2f), Vector2.down, slopeCheckDistance, whatIsGround);
                if (hit)
                {
                    float slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
                    if (slopeDownAngle != slopeDownAngleOld)
                    {
                        isOnSlope = true;
                        slopeDownAngleOld = slopeDownAngle;
                    }
                    Debug.DrawRay(hit.point, hit.normal, Color.green);
                }
                return isOnSlope;
            }

        }

        public bool CanWalkOnSlope(Vector2 checkPos, Vector2 transformRight, float slopeCheckDistance, float maxSlopeAngle, LayerMask whatIsGround)
        {
            bool canWalkOnSlope;
            float slopeDownAngle = 0 ;
            RaycastHit2D hit = Physics2D.Raycast(checkPos + new Vector2(0, .2f), Vector2.down, slopeCheckDistance, whatIsGround);
            if (hit)
            {
                Vector2 slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized * transformRight.x;
                slopeDownAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeDownAngle != slopeDownAngleOld)
                {
                    slopeDownAngleOld = slopeDownAngle;
                }
                Debug.DrawRay(hit.point, hit.normal, Color.red);
                Debug.DrawRay(hit.point, slopeNormalPerp, Color.red);
            }
            if (slopeDownAngle > maxSlopeAngle || slopeSideAngle > maxSlopeAngle)
            {
                canWalkOnSlope = false;
            }
            else
            {
                canWalkOnSlope = true;
            }

            return canWalkOnSlope;
        }

        public Vector2 GetSlopeNormalPerp(Vector2 checkPos, Vector2 direction, float slopeCheckDistance, float maxSlopeAngle, LayerMask whatIsGround)
        {
            RaycastHit2D hit = Physics2D.Raycast(checkPos + new Vector2(0, .2f), Vector2.down, slopeCheckDistance, whatIsGround);
            if (hit)
            {
                Vector2 slopeNormalPerp = Vector2.Perpendicular(hit.normal).normalized * direction.x;
                return slopeNormalPerp;
            }
            return new Vector2(0, 0);
        }
    }
}


