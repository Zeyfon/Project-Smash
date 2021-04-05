using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;
using PSmash.Control;


namespace PSmash.Core
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] GameObject enemy = null;
        // Start is called before the first frame update

        public EnemyHealth SpawnEnemyEnableAutoAttackAndGetHealth(ParticleSystem spawnParticles)
        {
            SpawneParticles(spawnParticles);
            GameObject enemyClone = SpawnAndGetEnemy();
            return enemyClone.GetComponentInChildren<EnemyHealth>();
        }

        private GameObject SpawnAndGetEnemy()
        {
            GameObject enemyClone = Instantiate(enemy,transform.position,Quaternion.identity, transform);
            print(enemyClone);
            return enemyClone;
        }

        private void SpawneParticles(ParticleSystem spawnParticles)
        {
            ParticleSystem spawnParticlesClone = Instantiate(spawnParticles, transform.position, Quaternion.identity);
            StartCoroutine(ParticlesTracker(spawnParticlesClone));
        }

        IEnumerator ParticlesTracker(ParticleSystem spawnParticles)
        {
            while (spawnParticles.IsAlive())
            {
                yield return null;
            }
            Destroy(spawnParticles.gameObject);
        }
    }

}
