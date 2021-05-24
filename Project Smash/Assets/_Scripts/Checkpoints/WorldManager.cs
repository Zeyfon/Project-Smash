using PSmash.Inventories;
using PSmash.SceneManagement;
using System.Collections;
using UnityEngine;

namespace PSmash.Checkpoints
{
    public class WorldManager : MonoBehaviour
    {
        //STATE
        static int checkpointCounter = 0;

        //INITIALIZE
        private void OnEnable()
        {
            Tent.OnCheckpointDone += Tent_OnCheckpointDone;
        }

        private void OnDisable()
        {
            Tent.OnCheckpointDone -= Tent_OnCheckpointDone;
        }

        /////////////////////////////////////////PUBLIC//////////////////////////////////////
        public int GetCheckpointCounter()
        {
            return checkpointCounter;
        }

        public void IncreaseCheckpointCounter()
        {
            checkpointCounter++;
            print("Checkpoint Counter is  " + checkpointCounter);
        }

        /////////////////////////////////////////////PRIVATE/////////////////////////////////     
        void Tent_OnCheckpointDone()
        {
            IncreaseCheckpointCounter();
            //ClearLists();
            ResetObjects();
            Save();
        }

        void ResetObjects()
        {
            StartCoroutine(ResetObjects_CR());
        }

        IEnumerator ResetObjects_CR()
        {
            DestroyAllDamagingObjects();
            yield return GetComponentInChildren<ResetDestructibleObjects>().ResetDestructibleObjects_CR();
            yield return GetComponentInChildren<EnemiesReset>().ResetEnemies();
            yield return GetComponentInChildren<EnvironmentObjectsManager>().ResetEnvironmentalObjects();
        }

        void DestroyAllDamagingObjects()
        {
            foreach (Projectile projectile in FindObjectsOfType<Projectile>())
            {
                projectile.InstantDestroy();
            }
        }

        void Save()
        {
            FindObjectOfType<SavingWrapper>().Save();
        }
    }

}
