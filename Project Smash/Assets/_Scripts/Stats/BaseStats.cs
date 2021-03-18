using UnityEngine;
using PSmash.Attributes;
using PSmash.LevelUpSystem;
using PSmash.Items;
using System;

namespace PSmash.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] float healthValue = 100;
        [SerializeField] float damage = 10;
        [Tooltip("The defense is a percentage")]
        [SerializeField] float defense = 10;

        PlayerHealth health;

        private void Start()
        {
            health = GetComponent<PlayerHealth>();
        }

        public float GetStat(StatsList stat)
        {
            switch (stat)
            {
                case StatsList.Health:
                    return healthValue;
                case StatsList.Defense:
                    return defense;
                case StatsList.Damage:
                    return damage;
                default:
                    return 0;
            }
        }
        public void SetStat(StatsList stat, float value)
        {
            switch (stat)
            {
                case StatsList.Defense:
                    defense = value;
                    break;
                default:
                    break;
            }
        }

        public void UnlockSkill(Skill skill)
        {
            switch (skill.stat)
            {
                case StatsList.Health:
                    float extraHealthValue = Mathf.Round(healthValue*(skill.value / 100));
                    healthValue += extraHealthValue;
                    health.RestoreHealth(extraHealthValue);
                    //print(healthValue);
                    break;
                case StatsList.Damage:
                    float extraDamage = Mathf.Round(damage * (skill.value / 100));
                    damage = damage + extraDamage;
                    //print(damage);
                    break;
                case StatsList.Defense:
                    float extraDefense = Mathf.Round(defense * (skill.value / 100));
                    defense = defense + extraDefense;
                    //print(damage);
                    break;
                default:
                    break;
            }
        }
    }

}
