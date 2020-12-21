using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Core
{
    public class BeatenUpDoor : MonoBehaviour
    {

        private void Awake()
        {
            OpenDoor();

        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void CloseDoor()
        {
            GetComponent<Collider2D>().enabled = true;
            GetComponentInChildren<SpriteRenderer>().enabled = true;
        }

        public void OpenDoor()
        {
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
        }
    }

}
