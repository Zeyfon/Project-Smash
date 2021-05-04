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
        static int checkpointCounter = 0;

        public void Interact()
        {
            if (OnTentMenuOpen != null)
            {
                print("Interactin with Tent");
                OnTentMenuOpen();
                OnCheckpointDone();
                StartCoroutine(CheckpointReset());
            }
        }

        public int GetCheckpointCounter()
        {
            return checkpointCounter;
        }

        public bool IsPlayerInSavePoint()
        {
            return isPlayerInSavePoint;
        }

        IEnumerator CheckpointReset()
        {
            checkpointCounter++;
            GameObject sceneManager = GameObject.FindGameObjectWithTag("SceneManager");
            DestroyAllDamagingObjects();
            yield return sceneManager.GetComponent<ResetDestructibleObjects>().ResetDestructibleObjects_CR();
            yield return sceneManager.GetComponent<EnemiesReset>().ResetEnemies();
            yield return sceneManager.GetComponent<EnvironmentObjectsManager>().ResetEnvironmentalObjects();
            FindObjectOfType<SavingWrapper>().Save();
        }

        static void DestroyAllDamagingObjects()
        {
            foreach (Projectile projectile in FindObjectsOfType<Projectile>())
            {
                projectile.InstantDestroy();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                print("Player inside Checkpoint");
                isPlayerInSavePoint = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                isPlayerInSavePoint = false;
                print("Player outside Checkpoint");
            }
        }
    }

}
