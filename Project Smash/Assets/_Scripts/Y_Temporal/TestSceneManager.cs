using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

namespace PSmash.Temporal
{
    public class TestSceneManager : MonoBehaviour
    {
        [SerializeField] GameObject enemy;
        [SerializeField] List<Transform> spawnPositions = new List<Transform>();

        //The  enemis add and removes from the list by themselves in the EnemyHealth Script
        public List<GameObject> enemiesAlive = new List<GameObject>();

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(CheckingEnemies());
        }

        IEnumerator CheckingEnemies()
        {
            while (true)
            {
                yield return new WaitForSeconds(2);
                if (enemiesAlive.Count < 3) StartCoroutine(CreateEnemy());
            }
        }
        IEnumerator CreateEnemy()
        {
            Instantiate(enemy, GetRandomSpawnPosition(), Quaternion.identity);
            yield return null;
        }

        Vector3 GetRandomSpawnPosition()
        {
            return spawnPositions[Random.Range(0, spawnPositions.Count)].position;
        }
    }
}

