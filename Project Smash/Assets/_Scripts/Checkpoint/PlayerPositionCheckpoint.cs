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
            if (canSavePosition)
            {
                print(gameObject.name + "  capturing player position");
                SerializableVector3 position = new SerializableVector3(transform.position);
                return position;
            }

            return null;
        }

        
        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            if (position == null)
                return;
            Vector3 newPosition = position.ToVector();
            FindObjectOfType<PlayerMovement>().transform.position = newPosition;
            print(gameObject.name + "  restoring player position");
            //FindObjectOfType<PlayerMovement>().GetComponent<Rigidbody2D>().MovePosition(newPosition);
        }
    }
}


