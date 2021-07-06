using UnityEngine;

namespace PSmash.Movement
{
    public class Ladder : MonoBehaviour
    {
        [SerializeField] Transform ladderTop = null;
        [SerializeField] bool isLadderOrientationToRight = true;
        private void Start()
        {
            SetPlatformYPosAutomatically();
        }

        private void SetPlatformYPosAutomatically()
        {
            BoxCollider2D myBoxCollider = GetComponent<BoxCollider2D>();
            BoxCollider2D playerGroundBoxCollider = ladderTop.transform.GetChild(0).GetComponent<BoxCollider2D>();
            float colliderUpperEdge = transform.position.y + myBoxCollider.size.y / 2 + myBoxCollider.offset.y;
            ladderTop.position = new Vector3(transform.position.x, colliderUpperEdge - playerGroundBoxCollider.size.y / 2, transform.position.z);
        }

        public bool GetLadderOrientation()
        {
            return isLadderOrientationToRight;
        }
    }
}

