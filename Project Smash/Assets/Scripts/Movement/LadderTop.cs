using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Movement
{
    public class LadderTop : MonoBehaviour
    {

        bool isInverted = false;

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                print("Player exited trigger");
            }
        }

        public void InvertPlatform()
        {
            print("Inverting Platform");
            GetComponent<PlatformEffector2D>().rotationalOffset = 180;
            isInverted = true;
        }
        public bool IsInverted
        {
            get
            {
                return isInverted;
            }
            set
            {
                isInverted = value;
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!isInverted) return;
            if (collision.collider.CompareTag("Player"))
            {
                //print("Player exited collision");
                GetComponent<PlatformEffector2D>().rotationalOffset = 0;
            }
        }
    }

}
