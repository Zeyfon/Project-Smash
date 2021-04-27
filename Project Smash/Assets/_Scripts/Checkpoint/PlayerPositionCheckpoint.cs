using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using PSmash.Attributes;

namespace PSmash.Checkpoints
{
    public class PlayerPositionCheckpoint : MonoBehaviour, ISaveable
    {

        bool canSaveCheckpoint = false;

        public bool CanSaveCheckpoint()
        {
            return canSaveCheckpoint;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                //print("Player Pos Can be saved");
                canSaveCheckpoint = true;
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                //print("Player Pos cannot be saved");
                canSaveCheckpoint = false;
            }
        }

        public object CaptureState()
        {
            if (canSaveCheckpoint)
            {
                //print(gameObject.name + "  capturing player position");
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
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            player.transform.position = newPosition;
            player.GetComponent<PlayerHealth>().RestoreHealth(99999);
            //print("Setting " + player.name + "   to Position  " + newPosition);
        }
    }
}


