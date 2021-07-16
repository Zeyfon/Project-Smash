using UnityEngine;
using System.Collections;

namespace PSmash.Movement
{
    public class OneWayPlatform : MonoBehaviour
    {
        [SerializeField] PlatformEffector2D effector = null;
        [SerializeField] Collider2D playerFloorCollider = null;
        bool canDetectExitCollision = false;

        public void RotatePlatform()
        {
            playerFloorCollider.enabled = false;
            //effector.rotationalOffset = 180;
            StartCoroutine(Timer());
            print("RotatePlatform");
        }

        IEnumerator Timer()
        {
            yield return new WaitForSeconds(0.5f);
            canDetectExitCollision = true;
            playerFloorCollider.enabled = true;
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (canDetectExitCollision && collision.collider.CompareTag("Player"))
            {
                effector.rotationalOffset = 0;
                canDetectExitCollision = false;
            }
        }
    }
}

