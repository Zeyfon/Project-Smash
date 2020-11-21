using PSmash.Items;
using UnityEngine;

namespace PSmash.Movement
{
    public class Ladder : MonoBehaviour
    {
        [SerializeField] Transform ladderTopTransform = null;
        PlayerMovement playerMovement;
        bool isPlayerAboveLadderTop;
        //The LadderTop children must be at the same position as the father for this to work fine.
        private void Start()
        {
            ladderTopTransform.position = new Vector3(transform.position.x,
                                                          transform.position.y + GetComponent<BoxCollider2D>().size.y / 2 - ladderTopTransform.GetChild(0).GetComponent<BoxCollider2D>().size.y / 2,
                                                          transform.position.z);
        }
        public void InvertPlatform()
        {
            transform.GetChild(0).GetComponent<LadderTop>().InvertPlatform();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;
            bool isLadderDetected = true;
            if (playerMovement == null) playerMovement = collision.GetComponent<PlayerMovement>();
            CheckPlayerRelativePositionToLadderTop(collision);
            playerMovement.SetIsLadderDetected(transform.position.x, isLadderDetected);
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;
            CheckPlayerRelativePositionToLadderTop(collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (!collision.CompareTag("Player")) return;
            bool isLadderDetected = false;
            if (playerMovement == null) playerMovement = collision.GetComponent<PlayerMovement>();
            playerMovement.SetIsLadderDetected(transform.position.x, isLadderDetected);
        }

        private void CheckPlayerRelativePositionToLadderTop(Collider2D collision)
        {
            if ((collision.transform.position.y - ladderTopTransform.position.y) > 0)
            {
                print("Player is above LadderTop");
                isPlayerAboveLadderTop = true;
            }
            else
            {
                print("Player is below LadderTop");

                isPlayerAboveLadderTop = false;
            }
            playerMovement.IsPlayerAboveLadderTop(isPlayerAboveLadderTop);
        }
    }
}

