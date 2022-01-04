using PSmash.Inventories;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.CraftingSystem
{
    [ExecuteAlways]
    public class SkillSlot : MonoBehaviour
    {
        //CONFIG
        [Header("CONFIG")]
        [SerializeField] Skill skill = null;
        [SerializeField] SkillSlot[] unlockableBySkillSlotsOptions;
        [SerializeField] CraftingItemSlot[] requiredCraftingItems;


        [Header("STATE")]
        [SerializeField] bool isUnlocked = false;

        public delegate void SkillPanelUpdate(SkillSlot slot);
        public event SkillPanelUpdate OnSkillPanelUpdate;

        bool canBeUnlocked = false;



        //STATE
        [System.Serializable]
        public class CraftingItemSlot
        {
            public CraftingItem item;
            public int number;
        }



        //INITIALIZE


        private void OnEnable()
        {
            CraftingSystem.OnSkillPanelUpdate += UpdateSkillslotSate;
            SetSkillSlotState(null);
        }

        private void OnDisable()
        {
            CraftingSystem.OnSkillPanelUpdate -= UpdateSkillslotSate;

        }
        private void UpdateSkillslotSate(SkillSlot slot)
        {
            SetSkillSlotState(slot);
        }


        //////////////////////////////////////////////////////////////////////PUBLIC/////////////////////////////////////////////////////////////////////

        public bool CanBeUnlocked()
        {
            if (!isUnlocked && canBeUnlocked)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public void SetIsUnlocked(bool isUnlocked)
        {
            this.isUnlocked = isUnlocked;
        }


        public Item GetItem()
        {
            return skill.GetItem();
        }

        public Skill GetSkill()
        {
            return skill;
        }

        public CraftingItemSlot[] GetRequiredCraftingItems()
        {
            return requiredCraftingItems;
        }

        public Dictionary<CraftingItem, int> GetCraftingItemsRequirement()
        {
            Dictionary<CraftingItem, int> requiredCraftingItems = new Dictionary<CraftingItem, int>();

            foreach (CraftingItemSlot requiredCraftingMaterial in this.requiredCraftingItems)
            {
                requiredCraftingItems.Add(requiredCraftingMaterial.item, requiredCraftingMaterial.number);
            }
            return requiredCraftingItems;
        }

        /// <summary>
        /// Checks if the skillSlot has been unlocked
        /// </summary>
        /// <returns></returns>
        public bool IsUnlocked()
        {
            return isUnlocked;
        }





        //////////////////////////////////////////////////////////////////////PRIVATE/////////////////////////////////////////////////////////////////////


        void SetSkillSlotState(SkillSlot slot)
        {
            SetCanBeUnlocked();

            // This event alert the Script in charge of the visuals of this skillslot 
            // to update the state
            if (OnSkillPanelUpdate != null)
            {
                OnSkillPanelUpdate(slot);
            }
        }

        private void SetCanBeUnlocked()
        {
            if (IsAnyPreviousSkillSlotUnlocked() && HasRequiredMaterialsToUnlock())
            {
                canBeUnlocked = true;
            }
            else
            {
                canBeUnlocked = false;
            }
        }

        bool IsAnyPreviousSkillSlotUnlocked()
        {
            if (unlockableBySkillSlotsOptions.Length == 0)
            {
                return true;
            }

            foreach (SkillSlot slot in unlockableBySkillSlotsOptions)
            {
                if (slot.IsUnlocked())
                {
                    return true;
                }
            }

            return false;
        }

        bool HasRequiredMaterialsToUnlock()
        {
            Dictionary<CraftingItem, int> itemsNeeded = new Dictionary<CraftingItem, int>();
            foreach(CraftingItemSlot slot in requiredCraftingItems)
            {
                int numberNeeded = Inventory.GetPlayerInventory().GetThisCraftingItemNumber(slot.item);
                if (numberNeeded >= slot.number)
                {
                    continue;
                }
                else
                {
                    //print("Items Criteria is not fulfilled to unlock skill");
                    return false;
                }
            }
            return true;
        }


    }

}
