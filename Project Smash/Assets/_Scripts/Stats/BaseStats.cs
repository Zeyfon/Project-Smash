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
                if(slot.stat == stat)
                {
                    return slot.number;
                }
            }
            Debug.LogWarning("No Stat was loaded. A 0 will be returned for  " + stat);
            return 0;
        }
        public void SetStat(StatsList stat, float value)
        {
            foreach(StatSlot slot in slots)
            {
                if(slot.stat == stat)
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
                if(slot.stat == stat.GetMyEnumID())
                {
                    float extraHealthValue = Mathf.Round(slot.number * (stat.GetNumber() / 100));
                    slot.number += extraHealthValue;
                    if(slot.stat == StatsList.Health)
                    {
                        health.RestoreHealth(extraHealthValue);
                    }
                    return;
                }
            }
        }

        public object CaptureState()
        {
            Dictionary<StatsList, float> data = new Dictionary<StatsList, float>();
            foreach(StatSlot slot in slots)
            {
                data.Add(slot.stat, slot.number);
            }
            return data;
        }

        public void RestoreState(object state)
        {
            Dictionary<StatsList, float> data = (Dictionary<StatsList, float>)state;
            foreach(StatsList stat in data.Keys)
            {
                foreach(StatSlot slot in slots)
                {
                    if(slot.stat == stat)
                    {
                        slot.number = data[stat];
                    }
                }
            }
            health.RestoreHealth(999);
        }

        [System.Serializable]
        public class StatSlot
        {
            [Tooltip("The stat to consider")]
            public StatsList stat;
            [Tooltip("The amount that will be used as default value in case no loading has been done")]
            public float number;
        }
    }

}
