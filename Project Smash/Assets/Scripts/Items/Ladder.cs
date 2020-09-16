using PSmash.Items;
using UnityEngine;

namespace PSmash.Movement
{
    public class Ladder : MonoBehaviour
    {
        [SerializeField] Transform ladderTopTransform = null;
        PlayerMovementV2 playerMovement;
        bool isPlayerAboveLadderTop;
        // Start is called before the first frame update

        public void InvertPlatform()
        {
            transform.GetChild(0).GetComponent<LadderTop>().InvertPlatform();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            bool isLadderDetected = true;
            if (playerMovement == null) playerMovement = collision.GetComponent<PlayerMovementV2>();
            CheckPlayerRelativePositionToLadderTop(collision);
            playerMovement.SetIsLadderDetected(transform.position.x, isLadderDetected);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            CheckPlayerRelativePositionToLadderTop(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            bool isLadderDetected = false;
            if (playerMovement == null) playerMovement = collision.GetComponent<PlayerMovementV2>();
            playerMovement.SetIsLadderDetected(transform.position.x, isLadderDetected);
        }

        private void CheckPlayerRelativePositionToLadderTop(Collider2D collision)
        {
            if ((collision.transform.position.y - ladderTopTransform.position.y) > 0)
            {
                isPlayerAboveLadderTop = true;
            }
            else
            {
                isPlayerAboveLadderTop = false;
            }
            playerMovement.IsPlayerAboveLadderTop(isPlayerAboveLadderTop);
        }
    }
}

