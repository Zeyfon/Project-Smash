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
    public class BeatenUpMoment : MonoBehaviour
    {
        [SerializeField] GameObject enemy = null;
        [SerializeField] CinemachineVirtualCamera vCam = null;
        [SerializeField] BeatenUpDoor[] doors = null;
        [SerializeField] Transform[] spawners = null;

        List<EnemyHealth> health = new List<EnemyHealth>();
        bool enemiesAlive = true;
        public void StartBeatenUpMoment(Transform target)
        {
            //Set the camera to cover the whole area of the Beaten Up Combat
            //  (Use Cinemachine blend with the priority property for a smooth blend)
            vCam.m_Priority = 20;
            //Close the doors fast enough to not let the player get out of the area.
            //  (Use another script that will be in charge of the whole closing and opening door process)
            foreach (BeatenUpDoor door in doors)
            {
                door.CloseDoor();
            }
            //Wait 2 seconds
            //Spawn the enemies in the air letting them fall to the ground.
            // (Set defined positions in the area that the enemies will use to spawn)
            StartCoroutine(SpawnEnemies(target));
        }

        IEnumerator SpawnEnemies(Transform target)
        {
            yield return new WaitForSeconds(2);
            foreach(Transform spawner in spawners)
            {
                GameObject enemyClone = Instantiate(enemy, spawner.position, Quaternion.identity);
                print(enemyClone);
                enemyClone.GetComponentInChildren<EnemyController>().EnableRageState();
                health.Add(enemyClone.GetComponentInChildren<EnemyHealth>());
            }
            StartCoroutine(CheckForEnemyDeads());
        }

        IEnumerator CheckForEnemyDeads()
        {
            while (true)
            {
                enemiesAlive = false;
                //Will be on the look out for enemies alive
                for(int i = 0; i <health.Count; i++)
                {
                    if (!health[i].IsDead()) enemiesAlive = true;
                }

                //Will check by the end of the for loop for alive enemis
                //If there are it will repeat the while loop
                if (enemiesAlive)
                {
                    //print("Enemies are still alive");
                    yield return null;
                }

                //If not it will start the open doors process and will stop this coroutine
                else
                {
                    StartCoroutine(EndsBeatenUpMoment());
                    yield break;
                }
            }
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

        void SpawnEffects()
        {
            //Play the effects you might like 
        }

    }

}
