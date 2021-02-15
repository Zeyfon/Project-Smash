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
        [SerializeField] float potionsUses = 1;

        [Header("Materials")]
        [SerializeField] int monsterRemains = 888;
        [SerializeField] int reapersEye = 777;
        [SerializeField] int rangersEye = 666;
        [SerializeField] int smasherEye = 0;
        [SerializeField] int spiderEye = 0;
        [SerializeField] int XXXXX = 0;
        [SerializeField] int wood = 0;
        [SerializeField] int rock = 0;

        PlayerHealth health;

        private void Start()
        {
            health = GetComponent<PlayerHealth>();
        }

        private void OnEnable()
        {
            EnemyDrop.onDropCollected += CollectDrop;
        }

        private void OnDisable()
        {
            EnemyDrop.onDropCollected -= CollectDrop;

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
                case StatsList.Potions:
                    return potionsUses;
                default:
                    return 0;
            }
        }
        public void SetStat(StatsList stat, float value)
        {
            switch (stat)
            {
                case StatsList.Potions:
                     potionsUses = value;
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
                    health.ReplenishHealth(extraHealthValue);
                    //print(healthValue);
                    break;
                case StatsList.Damage:
                    float extraDamage = Mathf.Round(damage * (skill.value / 100));
                    damage = damage + extraDamage;
                    //print(damage);
                    break;
                default:
                    break;
            }
        }

        public int GetMaterialQuantity(CraftingMaterialsList material)
        {
            switch (material)
            {
                case CraftingMaterialsList.MonsterRemains:
                    return monsterRemains;
                case CraftingMaterialsList.RangerEye:
                    return rangersEye;
                case CraftingMaterialsList.ReaperEye:
                    return reapersEye;
                case CraftingMaterialsList.SmashserEye:
                    return smasherEye;
                case CraftingMaterialsList.SpiderEye:
                    return spiderEye;
                case CraftingMaterialsList.XXXXX:
                    return XXXXX;
                case CraftingMaterialsList.Wood:
                    return wood;
                case CraftingMaterialsList.Rock:
                    return rock;
                default:
                    return 0 ;
            }
        }

        private void CollectDrop(CraftingMaterialsList material)
        {
            //print("Player collected  1 "  + material.ToString());
            UpdateMaterialPossessedByPlayer(material, 1);
        }

        public void UpdateMaterialPossessedByPlayer(CraftingMaterialsList material, int value)
        {
            //print(material + "  value  " + value);
            switch (material)
            {
                case CraftingMaterialsList.MonsterRemains:
                    monsterRemains +=value;
                    break;
                case CraftingMaterialsList.RangerEye:
                    rangersEye +=value;
                    break;
                case CraftingMaterialsList.ReaperEye:
                    reapersEye += value;
                    break;
                case CraftingMaterialsList.SmashserEye:
                    smasherEye += value;
                    break;
                case CraftingMaterialsList.SpiderEye:
                    spiderEye += value;
                    break;
                case CraftingMaterialsList.XXXXX:
                    XXXXX += value ;
                    break;
                case CraftingMaterialsList.Wood:
                    wood += value;
                    break;
                case CraftingMaterialsList.Rock:
                    rock += value;
                    break;
                default:
                    break;
            }
        }
    }

}
