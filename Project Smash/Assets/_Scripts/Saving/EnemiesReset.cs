using GameDevTV.Saving;
using PSmash.Attributes;
using PSmash.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PSmash.Checkpoints
{
    public class EnemiesReset : MonoBehaviour
    {
        //CONFIG
        [SerializeField] GameObject reaperPrefab = null;
        [SerializeField] GameObject rangerPrefab = null;
        [SerializeField] GameObject smasherPrefab = null;

        //STATE
        struct EnemySlot
        {
            public EnemyID id;
            public Vector2 position;
            public bool isLookingRight;
            public EnemyID.EnemyType enemyType;
            public string identifier;
        }

        List<EnemySlot> slots = new List<EnemySlot>();
        ////////////////////////////////////////////////////////////////////INITIALIZE/////////////////////////////////////////////////////////////////////////////
        // Start is called before the first frame update
        void Awake()
        {
            SetEnemyRecord();

        }

        ///////////////////////////////////////////////////////////////////////PUBLIC/////////////////////////////////////////////////////////////////////////
        public IEnumerator ResetEnemies()
        {
            EnemyHealth.takenOutEnemies.Clear();
            foreach (EnemySlot slot in slots)
            {
                slot.id.gameObject.SetActive(false);
                Destroy(slot.id.gameObject,1);
                GameObject prefabClone = Instantiate(GetPrefab(slot), slot.position, Quaternion.identity, transform);
                prefabClone.GetComponentInChildren<SaveableEntity>().OverwriteUniqueIdentifer(slot.identifier);
            }
            SetEnemyRecord();
            print("Enemies respawned");
            yield return null;
        }


        ///////////////////////////////////////////////////////////////////////PRIVATE/////////////////////////////////////////////////////////////////////////
        void SetEnemyRecord()
        {
            slots.Clear();
            foreach (EnemyID id in FindObjectsOfType<EnemyID>())
            {
                EnemySlot slot = new EnemySlot();
                slot.id = id;
                slot.position = id.transform.position;
                slot.isLookingRight = true;
                slot.enemyType = id.GetEnemyType();
                string identifier = id.GetComponentInChildren<SaveableEntity>().GetUniqueIdentifier();
                slot.identifier = identifier;
                slots.Add(slot);
                //SetUniqueIdentifiers(identifier);
            }
        }

        //void SetUniqueIdentifiers(string identifier)
        //{
        //    if (!identifiers.Contains(identifier))
        //    {
        //        identifiers.Add(identifier);
        //        Debug.Log("Identifier was added  " + identifier);
        //    }
           
        //}


        GameObject GetPrefab(EnemySlot slot)
        {
            GameObject prefab = null;
            if (slot.enemyType == EnemyID.EnemyType.Ranger)
            {
                prefab = rangerPrefab;
            }
            else if (slot.enemyType == EnemyID.EnemyType.Reaper)
            {
                prefab = reaperPrefab;
            }
            else if (slot.enemyType == EnemyID.EnemyType.Smasher)
            {
                prefab = smasherPrefab;
            }
            return prefab;
        }
    }

}

