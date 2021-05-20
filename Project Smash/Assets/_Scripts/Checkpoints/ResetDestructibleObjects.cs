using System.Collections.Generic;
using UnityEngine;
using PSmash.Items;
using System.Collections;
using GameDevTV.Saving;

namespace PSmash.Checkpoints
{
    public class ResetDestructibleObjects : MonoBehaviour
    {

        //CONFIG
        [SerializeField] GameObject barrelPrefab = null;
        [SerializeField] GameObject cratePrefab = null;
        [SerializeField] GameObject rockPrefab = null;

        //STATE
        struct ObjectSlot
        {
            public DestructibleObject destructibleObject;
            public Vector2 position;
            public string identifier;
        }

        List<ObjectSlot> slots = new List<ObjectSlot>();

        //INITIALIZE
        void Awake()
        {
            SetObjectsRecord();
        }

        //////////////////////////////////////////////////////PUBLIC///////////////////////////////////////////////////

        public IEnumerator ResetDestructibleObjects_CR()
        {
            foreach (ObjectSlot slot in slots)
            {
                slot.destructibleObject.gameObject.SetActive(false);
                Destroy(slot.destructibleObject.gameObject, 1);
                GameObject clone = Instantiate(GetObjectPrefab(slot), slot.position, Quaternion.identity, transform);
                clone.GetComponentInChildren<SaveableEntity>().OverwriteUniqueIdentifer(slot.identifier);
            }
            SetObjectsRecord();
            //print("Enemies respawned");
            yield return null;
        }


        ///////////////////////////////////////////////////PRIVATE//////////////////////////////////////////////////////
        void SetObjectsRecord()
        {
            slots.Clear();
            foreach (DestructibleObject obj in FindObjectsOfType<DestructibleObject>())
            {

                ObjectSlot slot = new ObjectSlot();
                slot.destructibleObject = obj;
                slot.position = obj.transform.position;
                string identifier = obj.GetComponentInChildren<SaveableEntity>().GetUniqueIdentifier();
                slot.identifier = identifier;
                slots.Add(slot);
            }
        }

        GameObject GetObjectPrefab(ObjectSlot slot)
        {
            GameObject prefab = null;
            if (slot.destructibleObject.GetObjectType() == DestructibleObject.ObjectType.barrel)
            {
                prefab = barrelPrefab;
            }
            if (slot.destructibleObject.GetObjectType() == DestructibleObject.ObjectType.crate)
            {
                prefab = cratePrefab;
            }
            if (slot.destructibleObject.GetObjectType() == DestructibleObject.ObjectType.rock)
            {
                prefab = rockPrefab;
            }
            return prefab;
        }

    }
}

