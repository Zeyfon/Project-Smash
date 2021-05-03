using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using PSmash.Attributes;

namespace PSmash.Checkpoints
{
    public class PlayerPositionCheckpoint : MonoBehaviour, ISaveable
    {

        bool isPlayerInSavePoint = false;

        public bool IsPlayerInSavePoint()
        {
            return isPlayerInSavePoint;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                //print("Player Pos Can be saved");
                isPlayerInSavePoint = true;
            }

        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                //print("Player Pos cannot be saved");
                isPlayerInSavePoint = false;
            }
        }

        public object CaptureState()
        {
            if (isPlayerInSavePoint)
            {
                print(gameObject.name + "  capturing player position");
                SerializableVector3 position = new SerializableVector3(transform.position);
                return position;
            }

            return null;
        }

        
        public void RestoreState(object state, bool isLoadLastSavedScene)
        {
            SerializableVector3 position = (SerializableVector3)state;
            if (position == null)
            {
                Debug.LogWarning("Player not restored in save point");
                return;

            }
            Vector3 newPosition = position.ToVector();
            //GameObject player = GameObject.FindGameObjectWithTag("Player");
            //player.transform.position = newPosition;
            //player.GetComponent<PlayerHealth>().RestoreHealth(99999);
            //print("Setting " + player.name + "   to Position  " + newPosition);
        }
    }
}


