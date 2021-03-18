using UnityEngine;
using PSmash.LevelUpSystem;
using PSmash.Items;

namespace PSmash.Stats
{
    public class MyCraftingMaterials : MonoBehaviour
    {

        [Header("Materials")]
        [SerializeField] int monsterRemains = 888;
        [SerializeField] int reapersEye = 777;
        [SerializeField] int rangersEye = 666;
        [SerializeField] int smasherEye = 0;
        [SerializeField] int spiderEye = 0;
        [SerializeField] int XXXXX = 0;
        [SerializeField] int wood = 0;
        [SerializeField] int rock = 0;

        private void OnEnable()
        {
            EnemyDrop.onDropCollected += CollectDrop;
        }

        private void OnDisable()
        {
            EnemyDrop.onDropCollected -= CollectDrop;

        }

        public int GetPlayerQuantityForThisMaterial(LevelUpSystem.CraftingMaterial material)
        {
            return GetMaterialQuantity(CraftingMaterialsList.RangerEye);
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
            print("Player collected  1 "  + material.ToString());
            UpdateMyMaterials(material, 1);
        }

        public void UpdateMyMaterials(CraftingMaterialsList material, int value)
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
