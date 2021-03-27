using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using PSmash.Movement;

namespace PSmash.Checkpoints
{
    public class PlayerPositionCheckpoint : MonoBehaviour, ISaveable
    {

        bool canSavePosition = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                //print("Player Pos Can be saved");
                canSavePosition = true;
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                //print("Player Pos cannot be saved");
                canSavePosition = false;
            }
        }

        public object CaptureState()
        {
            SerializableVector3 position = new SerializableVector3(transform.position);
            return position;
        }
        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            Vector3 newPosition = position.ToVector();
            FindObjectOfType<PlayerMovement>().GetComponent<Rigidbody2D>().MovePosition(newPosition);
        }



        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}


