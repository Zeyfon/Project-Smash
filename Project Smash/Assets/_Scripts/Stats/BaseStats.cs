using GameDevTV.Saving;
using PSmash.Attributes;
using PSmash.Inventories;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Stats
{
    public class BaseStats : MonoBehaviour, ISaveable
    {

        [SerializeField] StatSlot[] slots;
        PlayerHealth health;

        private void Awake()
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

        public object CaptureState()
        {
            Dictionary<string, float> data = new Dictionary<string, float>();
            foreach(StatSlot slot in slots)
            {
                data.Add(slot.item.GetID(), slot.number);
            }
            return data;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, float> data = (Dictionary<string, float>)state;
            foreach(string itemID in data.Keys)
            {
                foreach(StatSlot slot in slots)
                {
                    if(slot.item.GetID() == itemID)
                    {
                        slot.number = data[itemID];
                    }
                }
            }
            health.RestoreHealth(999);
        }

        [System.Serializable]
        public class StatSlot
        {
            public StatusItem item;
            public float number;
        }
    }

}
