using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using PSmash.Attributes;
using PSmash.Control;

namespace PSmash.Core
{
    public class BeatenUpAreaController : MonoBehaviour, IAutomaticInteraction
    {
        //CONFIG
        [SerializeField] Collider2D combatEnabler = null;
        [SerializeField] CinemachineVirtualCamera vCam = null;
        [SerializeField] ParticleSystem spawnParticles = null;
        [SerializeField] Waves[] waves;

        //STATE
        int index = 0;
        int enemyQuantity = 0;
        int wavesLength = 0;
        //int currentCounter = 0;

        [System.Serializable]
        class Waves
        {
            [SerializeField] GameObject[] enemies;
        }

        //INITIALIZE
        void Awake()
        {
            wavesLength = GetComponentInChildren<EnemySpawner>().GetEnemiesWavesLength();
        }

        void OnDisable()
        {
            EnemyHealth.onEnemyDead -= EnemyDied;
        }


        /////////////////////////////////////////////////////////////////////////PUBLIC//////////////////////////////////////////////////////////////////////////
        public void Interact()
        {
            combatEnabler.enabled = false;
            print("Combat Starts");
            StartBeatenUpMoment();
        }

        /// <summary>
        /// Start the event of the beaten up area
        /// Also subscribe to the event within the EnemyHealth script onEnemyDead to know about the taken out enemies within the beaten up area
        /// </summary>
        public void StartBeatenUpMoment()
        {
            
            vCam.m_Priority = 20;
            foreach (BeatenUpDoor door in GetComponentsInChildren<BeatenUpDoor>())
            {
                door.CloseDoor();
            }
            EnemyHealth.onEnemyDead += EnemyDied;
            StartCoroutine(SpawnEnemies(0));
            GameObject.FindObjectOfType<MusicManager>().PlayBossMusic();
        }


        /////////////////////////////////////////////////////////////////////////////PRIVATE////////////////////////////////////////////////////////////////////////////////


        void EnemyDied()
        {
            enemyQuantity--;
            print("Nice!! 1 less. Take out " + enemyQuantity + " more and you are out");
        }

        IEnumerator SpawnEnemies(int index)
        {
            //print("SpawneEnemies");
            List<EnemyHealth> healths = new List<EnemyHealth>();
            yield return new WaitForSeconds(2);
            foreach(EnemySpawner spawner in GetComponentsInChildren<EnemySpawner>())
            {
                EnemyHealth health = spawner.SpawnEnemyEnableAutoAttackAndGetHealth(spawnParticles, index);
                if(health != null)
                    healths.Add(health);
                enemyQuantity++;
            }

            if (healths.Count == 0)
            {
                Debug.LogWarning("No enemies were added to the array");
            }

            while (enemyQuantity != 0)
            {
                yield return null;
            }

            index++;
            if (index >= wavesLength)
            {
                StartCoroutine(EndsBeatenUpMoment());
            }
            else
            {
                StartCoroutine(SpawnEnemies(index));
                //START AGAIN THE SPAWN ACTION WITH THE CHECKUP TO KNOW WHEN THE ENEMIES HAVE BEEN TAKEN OUT
            }
        }

        IEnumerator EndsBeatenUpMoment()
        {
            yield return new WaitForSeconds(2);
            GameObject.FindObjectOfType<MusicManager>().PlayLevelMusic();
            print("Enemies are not alive anymore");
            vCam.m_Priority = 0;
            foreach (BeatenUpDoor door in GetComponentsInChildren<BeatenUpDoor>())
            {
                door.OpenDoor();
            }
        }
    }

}
