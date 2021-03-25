using System;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;
using PSmash.Checkpoints;
using System.Collections;

namespace PSmash.Saving
{
    public class EnemiesReset : MonoBehaviour
    {
        [SerializeField] GameObject reaperPrefab = null;
        [SerializeField] GameObject rangerPrefab = null;
        [SerializeField] GameObject smasherPrefab = null;

        struct EnemySlot
        {
            public GameObject prefab;
            public Vector2 position;
            public bool isLookingRight;
            public EnemyID.EnemyType enemyType;
        }

        List<EnemySlot> slots = new List<EnemySlot>();
        // Start is called before the first frame update
        void Awake()
        {
            foreach(EnemyID id in GetComponentsInChildren<EnemyID>())
            {
                GameObject enemy = null;
                if(id.GetEnemyType() == EnemyID.EnemyType.Ranger)
                {
                    enemy = rangerPrefab;
                }
                if (id.GetEnemyType() == EnemyID.EnemyType.Smasher)
                {
                    enemy = smasherPrefab;
                }
                if (id.GetEnemyType() == EnemyID.EnemyType.Reaper)
                {
                    enemy = reaperPrefab;
                }
                EnemySlot slot = new EnemySlot();
                slot.prefab = enemy;
                slot.position = id.transform.position;
                slot.isLookingRight = true;
                print(slot.prefab.name);
                slots.Add(slot);
            }
        }

        private void OnEnable()
        {
            Checkpoint.onCheckpointPerformed += RespawnAllEnemies;
        }

        private void OnDisable()
        {
            Checkpoint.onCheckpointPerformed -= RespawnAllEnemies;

        }

        void RespawnAllEnemies()
        {
            StartCoroutine(RespawnEnemies());

        }

        IEnumerator RespawnEnemies()
        {
            foreach (EnemyID entity in GetComponentsInChildren<EnemyID>())
            {
                Destroy(entity.gameObject);
            }
            print("Enemies destroyed");
            foreach (EnemySlot slot in slots)
            {
                Instantiate(slot.prefab, slot.position, Quaternion.identity, transform);
            }
            print("Enemies respawned");
            yield return null;
        }
    }

}

