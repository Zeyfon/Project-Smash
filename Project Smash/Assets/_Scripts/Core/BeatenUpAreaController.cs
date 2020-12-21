using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using PSmash.Attributes;
using PSmash.Control;

namespace PSmash.Core
{
    //Start the Beaten up Moment
    //Keep track of the remaining ememies alive
    //When no enemies are alive open the doors
    //Let the player go back to his journey
    public class BeatenUpAreaController : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera vCam = null;
        [SerializeField] BeatenUpDoor[] doors = null;
        [SerializeField] EnemySpawner[] spawners = null;

        public void StartBeatenUpMoment()
        {
            vCam.m_Priority = 20;
            foreach (BeatenUpDoor door in doors)
            {
                door.CloseDoor();
            }
            StartCoroutine(SpawnEnemies());
        }

        IEnumerator SpawnEnemies()
        {
            //print("SpawneEnemies");
            List<EnemyHealth> healths = new List<EnemyHealth>();
            yield return new WaitForSeconds(2);
            foreach(EnemySpawner spawner in spawners)
            {
                EnemyHealth health = spawner.SpawnEnemyEnableAutoAttackAndGetHealth();
                healths.Add(health);
            }

            foreach (EnemyHealth health in healths)
            {
                print(health);
            }
            StartCoroutine(EnemyAliveTracker(healths));
        }

        IEnumerator EnemyAliveTracker(List<EnemyHealth> healths)
        {
            //print("EnemyTracker");
            if (healths.Count == 0)
            {
                Debug.LogWarning("No enemies were added to the array"); 
            }

            while (true)
            {
                bool enemiesAlive = CheckForRemainingAliveEnemies(healths);

                if (enemiesAlive) yield return null;
                else
                {
                    StartCoroutine(EndsBeatenUpMoment());
                    yield break;
                }
            }
        }

        private static bool CheckForRemainingAliveEnemies(List<EnemyHealth> healths)
        {
            bool enemiesAlive = false;
            foreach (EnemyHealth health in healths)
            {
                if (!health.IsDead()) enemiesAlive = true;
            }
            return enemiesAlive;
        }

        IEnumerator EndsBeatenUpMoment()
        {
            yield return new WaitForSeconds(2);
            print("Enemies are not alive anymore");
            vCam.m_Priority = 0;
            foreach (BeatenUpDoor door in doors)
            {
                door.OpenDoor();
            }
        }
    }

}
