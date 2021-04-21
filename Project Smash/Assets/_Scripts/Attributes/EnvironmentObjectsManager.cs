using PSmash.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Checkpoints
{
    public class EnvironmentObjectsManager : MonoBehaviour
    {
        [SerializeField] GameObject boulderPrefab = null;

        struct ObjectSlot
        {
            public GameObject prefab;
            public Vector2 position;
        }

        List<ObjectSlot> slots = new List<ObjectSlot>();
        // Start is called before the first frame update
        void Awake()
        {
            foreach (Boulder obj in FindObjectsOfType<Boulder>())
            {

                ObjectSlot slot = new ObjectSlot();
                slot.prefab = boulderPrefab;
                slot.position = obj.transform.position;
                //print(slot.prefab.name);
                slots.Add(slot);
            }
        }

        private void OnEnable()
        {
            Checkpoint.onCheckpointPerformed += RespawnAllBoulders;
        }

        private void OnDisable()
        {
            Checkpoint.onCheckpointPerformed -= RespawnAllBoulders;

        }

        void RespawnAllBoulders()
        {
            StartCoroutine(RespawnBoulders());

        }

        IEnumerator RespawnBoulders()
        {
            Boulder.bouldersMoved.Clear();
            foreach (Boulder boulderInstance in FindObjectsOfType<Boulder>())
            {
                Destroy(boulderInstance.gameObject);
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

