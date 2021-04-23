﻿using GameDevTV.Saving;
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
            public Transform transform;
            public Vector3 position;
            public string identifier;
        }

        List<ObjectSlot> slots = new List<ObjectSlot>();
        // Start is called before the first frame update
        void Awake()
        {
            SetObjectsRecord();
        }

        public IEnumerator ResetEnvironmentalObjects()
        {
            Boulder.bouldersMoved.Clear();
            foreach (ObjectSlot slot in slots)
            {
                slot.transform.gameObject.SetActive(false);
                Destroy(slot.transform.gameObject, 1);
                GameObject clone = Instantiate(boulderPrefab, slot.position, Quaternion.identity, transform);
                clone.GetComponentInChildren<SaveableEntity>().OverwriteUniqueIdentifer(slot.identifier);
            }
            SetObjectsRecord();
            yield return null;
        }

        void SetObjectsRecord()
        {
            slots.Clear();
            foreach (Boulder obj in FindObjectsOfType<Boulder>())
            {
                ObjectSlot slot = new ObjectSlot();
                slot.transform = obj.transform;
                slot.position = obj.transform.position;
                slot.identifier = obj.GetComponentInChildren<SaveableEntity>().GetUniqueIdentifier();
                slots.Add(slot);
            }
        }
    }

}

