using UnityEngine;

namespace PSmash.Movement
{
    public class ThinPlatform : MonoBehaviour
    {
        PlatformEffector2D effector;
        PlayerMovement playerMovement;

        private void Awake()
        {
            effector = GetComponent<PlatformEffector2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (playerMovement == null) playerMovement = collision.collider.GetComponent<PlayerMovement>();
            playerMovement.IsCollidingWithThinPlatform = true;
            
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (playerMovement == null)
            {
                playerMovement = collision.collider.GetComponent<PlayerMovement>();
            }
            if (playerMovement.MoveThroughFloor == true)
            {
                effector.rotationalOffset = 180;
                GetComponent<Collider2D>().isTrigger = true;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (playerMovement == null) playerMovement = collision.collider.GetComponent<PlayerMovement>();
            playerMovement.IsCollidingWithThinPlatform = false;
            effector.rotationalOffset = 0;
            playerMovement.MoveThroughFloor = false;
            print("ExitContact with ThinPlatform");
            //Debug.Break();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (playerMovement == null) playerMovement = collision.GetComponent<PlayerMovement>();
            playerMovement.IsCollidingWithThinPlatform = false;
            effector.rotationalOffset = 0;
            playerMovement.MoveThroughFloor = false;
            GetComponent<Collider2D>().isTrigger = false;
            print("ExitContact with ThinPlatform");
            //Debug.Break();
        }
    }
}

