using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Core
{
    public class PersistantObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistantObjectPrefab = null;

        static bool hasSpawned = false;
        // Start is called before the first frame update
        void Awake()
        {
            if (hasSpawned)
                return;
            SpawnPersistantObjects();
            hasSpawned = true;
        }

        void SpawnPersistantObjects()
        {
            GameObject persistantObject = Instantiate(persistantObjectPrefab);
            DontDestroyOnLoad(persistantObject);
        }
    }

}
