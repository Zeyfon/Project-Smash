using GameDevTV.Saving;
using PSmash.Attributes;
using PSmash.Core;
using PSmash.Stats;
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


        //INITIALIZE
        void Awake()
        {
            SetResetRecord();
        }

        ///////////////////////////////////////////////////////////////////////PUBLIC/////////////////////////////////////////////////////////////////////////
        public IEnumerator ResetEnemies()
        {
            foreach (EnemySlot slot in slots)
            {
                if (slot.id == null)
                    continue;
                slot.id.gameObject.SetActive(false);
                Destroy(slot.id.gameObject,1f);
                GameObject prefabClone = Instantiate(GetPrefab(slot), slot.position, Quaternion.identity);
                yield return null;
                //print(prefabClone.name);
                prefabClone.GetComponentInChildren<SaveableEntity>().OverwriteUniqueIdentifer(slot.identifier);
                //print("current id  " + prefabClone.GetComponentInChildren<SaveableEntity>().GetUniqueIdentifier() + "  new id  " + slot.identifier);
            }
            yield return new WaitForSeconds(0.5f);
            SetResetRecord();
            //print("Enemies respawned");
            yield return null;
        }


        ///////////////////////////////////////////////////////////////////////PRIVATE/////////////////////////////////////////////////////////////////////////
        void SetResetRecord()
        {
            foreach (EnemyID id in FindObjectsOfType<EnemyID>())
            {
                EnemySlot slot = new EnemySlot();
                slot.id = id;
                slot.position = id.transform.position;
                slot.isLookingRight = true;
                slot.enemyType = id.GetEnemyType();
                slot.identifier = id.GetComponent<SaveableEntity>().GetUniqueIdentifier();
                print(slot.identifier);
                slots.Add(slot);
            }
        }


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

