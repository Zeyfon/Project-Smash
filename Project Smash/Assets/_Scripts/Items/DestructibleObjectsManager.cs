using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Saving;

namespace PSmash.Items
{
    public class DestructibleObjectsManager : MonoBehaviour
    {
        List<DestructibleObject> objects = new List<DestructibleObject>();
    // Start is called before the first frame update
        void Start()
        {
            foreach(DestructibleObject obj in GetComponentsInChildren<DestructibleObject>())
            {
                objects.Add(obj);
            }
        }

        private void OnEnable()
        {
            Tent.OnTentMenuOpen += RespawnObjects;
        }

        void RespawnObjects()
        {
            foreach(DestructibleObject obj in objects)
            {
                obj.Respawn();
            }
        }

    }
}

