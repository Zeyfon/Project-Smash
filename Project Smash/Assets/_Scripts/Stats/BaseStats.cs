using UnityEngine;
using PSmash.Attributes;
using PSmash.LevelUpSystem;

namespace PSmash.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] float healthValue = 100;
        [SerializeField] float damage = 10;
        [Tooltip("The defense is a percentage")]
        [SerializeField] float defense = 10;
        [Header("Materials")]
        [SerializeField] int monsterRemains = 888;
        [SerializeField] int reapersEye = 777;
        [SerializeField] int rangersEye = 666;

        PlayerHealth health;

        private void Start()
        {
            health = GetComponent<PlayerHealth>();
        }

        public float GetStat(Stat stat)
        {
            switch (stat)
            {
                case Stat.Health:
                    return healthValue;
                case Stat.Defense:
                    return defense;
                case Stat.Damage:
                    return damage;
                default:
                    return 0;
            }
        }

        public void UnlockSkill(Skill skill)
        {
            switch (skill.stat)
            {
                case Stat.Health:
                    float extraHealthValue = Mathf.Round(healthValue*(skill.value / 100));
                    healthValue += extraHealthValue;
                    health.ReplenishHealth(extraHealthValue);
                    //print(healthValue);
                    break;
                case Stat.Damage:
                    float extraDamage = Mathf.Round(damage * (skill.value / 100));
                    damage = damage + extraDamage;
                    //print(damage);
                    break;
                default:
                    break;
            }
        }

        public int GetMaterialQuantity(CraftingMaterials material)
        {
            switch (material)
            {
                case CraftingMaterials.MonsterRemains:
                    return monsterRemains;
                case CraftingMaterials.RangerEye:
                    return rangersEye;
                case CraftingMaterials.ReaperEye:
                    return reapersEye;
                default:
                    return 0 ;
            }
        }

        public void UpdateMaterialPossessedByPlayer(CraftingMaterials material, int value)
        {
            print(material + "  value  " + value);
            switch (material)
            {
                case CraftingMaterials.MonsterRemains:
                    monsterRemains-=value;
                    break;
                case CraftingMaterials.RangerEye:
                    rangersEye-=value;
                    break;
                case CraftingMaterials.ReaperEye:
                    reapersEye-= value;
                    break;
                default:
                    break;
            }
        }
    }

}
