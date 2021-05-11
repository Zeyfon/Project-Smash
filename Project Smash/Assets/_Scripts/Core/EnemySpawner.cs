using System.Collections;
using UnityEngine;
using PSmash.Attributes;

namespace PSmash.Core
{
    public class EnemySpawner : MonoBehaviour
    {

        //CONFIG
        [SerializeField] GameObject[] enemies = null;

        //////////////////////////////////////////////////////////////////////PUBLIC/////////////////////////////////////////////////////////////// 
        public int GetEnemiesWavesLength()
        {
            return enemies.Length;
        }

        public EnemyHealth SpawnEnemyEnableAutoAttackAndGetHealth(ParticleSystem spawnParticles, int index)
        {
            SpawneParticles(spawnParticles);
            GameObject enemyClone = SpawnAndGetEnemy(index);
            return enemyClone.GetComponentInChildren<EnemyHealth>();
        }

        ////////////////////////////////////////////////////////////////////////////////PRIVATE////////////////////////////////////////////////////////////////////////////

        GameObject SpawnAndGetEnemy(int index)
        {
            GameObject enemyClone = Instantiate(enemies[index],transform.position,Quaternion.identity, transform);
            print(enemyClone);
            return enemyClone;
        }

        void SpawneParticles(ParticleSystem spawnParticles)
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

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }

}
