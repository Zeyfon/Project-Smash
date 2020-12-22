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
        [SerializeField] GameObject spawnParticles = null;
        // Start is called before the first frame update

        public EnemyHealth SpawnEnemyEnableAutoAttackAndGetHealth()
        {
            SpawneParticles();
            GameObject enemyClone = SpawnAndGetEnemy();
            return enemyClone.GetComponentInChildren<EnemyHealth>();
        }

        private GameObject SpawnAndGetEnemy()
        {
            GameObject enemyClone = Instantiate(enemy,transform.position,Quaternion.identity, transform);
            return enemyClone;
        }

        private void SpawneParticles()
        {
            GameObject psClone = Instantiate(spawnParticles, transform.position, Quaternion.identity);
            ParticleSystem ps = psClone.GetComponent<ParticleSystem>();
            StartCoroutine(ParticlesTracker(ps));
        }

        IEnumerator ParticlesTracker(ParticleSystem ps)
        {
            while (ps.IsAlive())
            {
                yield return null;
            }
            Destroy(ps.gameObject);
        }
    }

}
