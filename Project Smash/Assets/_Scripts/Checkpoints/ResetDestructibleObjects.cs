using System.Collections.Generic;
using UnityEngine;
using PSmash.Items;
using System.Collections;

namespace PSmash.Checkpoints
{
    public class ResetDestructibleObjects : MonoBehaviour
    {
        [SerializeField] GameObject barrelPrefab = null;
        [SerializeField] GameObject cratePrefab = null;
        [SerializeField] GameObject rockPrefab = null;
        List<DestructibleObject> objects = new List<DestructibleObject>();

        struct ObjectSlot
        {
            public GameObject prefab;
            public Vector2 position;
        }

        List<ObjectSlot> slots = new List<ObjectSlot>();
    // Start is called before the first frame update
        void Awake()
        {
            foreach (DestructibleObject obj in GetComponentsInChildren<DestructibleObject>())
            {
                GameObject tempObj = null;
                if (obj.GetObjectType() == DestructibleObject.ObjectType.barrel)
                {
                    tempObj = barrelPrefab;
                }
                if (obj.GetObjectType() == DestructibleObject.ObjectType.crate)
                {
                    tempObj = cratePrefab;
                }
                if (obj.GetObjectType() == DestructibleObject.ObjectType.rock)
                {
                    tempObj = rockPrefab;
                }
                ObjectSlot slot = new ObjectSlot();
                slot.prefab = tempObj;
                slot.position = obj.transform.position;
                //print(slot.prefab.name);
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
            foreach (DestructibleObject entity in GetComponentsInChildren<DestructibleObject>())
            {
                Destroy(entity.gameObject);
            }
            //print("Enemies destroyed");
            foreach (ObjectSlot slot in slots)
            {
                Instantiate(slot.prefab, slot.position, Quaternion.identity, transform);
            }
            //print("Enemies respawned");
            yield return null;
        }

    }
}

