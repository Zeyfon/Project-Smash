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
    public class BeatenUpAreaController : MonoBehaviour, IAutomaticInteraction
    {
        [SerializeField] Collider2D combatEnabler = null;
        [SerializeField] CinemachineVirtualCamera vCam = null;
        [SerializeField] ParticleSystem spawnParticles = null;

        int enemyQuantity = 0;
        int currentCounter = 0;


        void OnDisable()
        {
            EnemyHealth.onEnemyDead -= EnemyDied;
        }

        public void StartBeatenUpMoment()
        {
            
            vCam.m_Priority = 20;
            foreach (BeatenUpDoor door in GetComponentsInChildren<BeatenUpDoor>())
            {
                door.CloseDoor();
            }
            EnemyHealth.onEnemyDead += EnemyDied;
            StartCoroutine(SpawnEnemies());
        }

        void EnemyDied()
        {
            enemyQuantity--;
            print("Nice!! 1 less. Take out " + enemyQuantity + " more and you are out");
        }

        IEnumerator SpawnEnemies()
        {
            //print("SpawneEnemies");
            List<EnemyHealth> healths = new List<EnemyHealth>();
            yield return new WaitForSeconds(2);
            foreach(EnemySpawner spawner in GetComponentsInChildren<EnemySpawner>())
            {
                EnemyHealth health = spawner.SpawnEnemyEnableAutoAttackAndGetHealth(spawnParticles);
                healths.Add(health);
                enemyQuantity++;
            }

            if (healths.Count == 0)
            {
                Debug.LogWarning("No enemies were added to the array");
            }

            while(enemyQuantity != 0)
            {
                yield return null;
            }

            StartCoroutine(EndsBeatenUpMoment());

            //foreach (EnemyHealth health in healths)
            //{
            //    print(health);
            //}
            //StartCoroutine(EnemyAliveTracker(healths));
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

        bool CheckForRemainingAliveEnemies(List<EnemyHealth> healths)
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
            foreach (BeatenUpDoor door in GetComponentsInChildren<BeatenUpDoor>())
            {
                door.OpenDoor();
            }
        }

        public void Interact()
        {
            combatEnabler.enabled = false;
            print("Combat Starts");
            StartBeatenUpMoment();
        }
    }

}
