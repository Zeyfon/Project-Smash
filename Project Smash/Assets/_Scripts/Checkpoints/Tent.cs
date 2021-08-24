using PSmash.Core;
using PSmash.Inventories;
using PSmash.SceneManagement;
using System;
using System.Collections;
using UnityEngine;

namespace PSmash.Checkpoints
{
    /// <summary>
    /// Only passes the IInterface to a event for the Tent Menu to answer to its call
    /// The interface is used as a way to detect interactable collider, like the Tent's case
    /// </summary>
    public class Tent : MonoBehaviour, IManualInteraction
    {
        public static event Action OnTentMenuOpen;
        public static event Action OnCheckpointDone;

        bool isPlayerInSavePoint = false;


        public void Interact()
        {
            if (OnTentMenuOpen != null)
            {
                //print("Interactin with Tent");
                OnTentMenuOpen();
                OnCheckpointDone();
            }
        }



        public bool IsPlayerInSavePoint()
        {
            return isPlayerInSavePoint;
        }





        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                //print("Player inside Checkpoint");
                isPlayerInSavePoint = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isPlayerInSavePoint = false;
                //print("Player outside Checkpoint");
            }
        }
    }

}
