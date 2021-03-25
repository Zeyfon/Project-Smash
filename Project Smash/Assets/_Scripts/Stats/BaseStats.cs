using UnityEngine;
using PSmash.Attributes;
using PSmash.CraftingSystem;
using PSmash.Inventories;
using System;

namespace PSmash.Stats
{
    public class BaseStats : MonoBehaviour
    {

        [SerializeField] StatSlot[] slots;
        PlayerHealth health;

        private void Start()
        {
            health = GetComponent<PlayerHealth>();
        }

        public float GetStat(StatsList stat)
        {
            foreach(StatSlot slot in slots)
            {
                if(slot.item.GetMyEnumID() == stat)
                {
                    return slot.number;
                }
            }
            return 0;
        }
        public void SetStat(StatsList stat, float value)
        {
            foreach(StatSlot slot in slots)
            {
                if(slot.item.GetMyEnumID() == stat)
                {
                    slot.number = value;
                }
            }
        }

        public void UnlockSkill(StatusItem stat)
        {
            print("Unlocking a skill of  " + stat);
            foreach(StatSlot slot in slots)
            {
                if(slot.item == stat)
                {
                    float extraHealthValue = Mathf.Round(slot.number * (stat.GetNumber() / 100));
                    slot.number += extraHealthValue;
                    if(slot.item.name == "Health")
                    {
                        health.RestoreHealth(extraHealthValue);
                    }
                    return;
                }
            }
        }

        [System.Serializable]
        public class StatSlot
        {
            public StatusItem item;
            public float number;
        }
    }

}
