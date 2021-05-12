using PSmash.Inventories;
using PSmash.SceneManagement;
using System.Collections;
using UnityEngine;

namespace PSmash.Checkpoints
{
    public class WorldManager : MonoBehaviour
    {

        static int checkpointCounter = 0;

        private void OnEnable()
        {
            Tent.OnCheckpointDone += Tent_OnCheckpointDone;
        }

        private void OnDisable()
        {
            Tent.OnCheckpointDone -= Tent_OnCheckpointDone;

        }

        private void Tent_OnCheckpointDone()
        {
            ClearLists();
            ResetObjects();
            Save();
        }

        private void ResetObjects()
        {
            StartCoroutine(ResetObjects_CR());
        }

        public int GetCheckpointCounter()
        {
            return checkpointCounter;
        }

        public void ResetWorld()
        {
            print("World is reset");
            //StartCoroutine(ResetWorld_CR());
        }

        public void ClearLists()
        {
            GetComponentInChildren<ResetDestructibleObjects>().ClearObjectsList();
            GetComponentInChildren<EnemiesReset>().ClearObjectsList();
            GetComponentInChildren<EnvironmentObjectsManager>().ClearObjectsList();
        }

        IEnumerator ResetObjects_CR()
        {
            checkpointCounter++;
            DestroyAllDamagingObjects();
            yield return GetComponentInChildren<ResetDestructibleObjects>().ResetDestructibleObjects_CR();
            yield return GetComponentInChildren<EnemiesReset>().ResetEnemies();
            yield return GetComponentInChildren<EnvironmentObjectsManager>().ResetEnvironmentalObjects();
        }

        static void DestroyAllDamagingObjects()
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
