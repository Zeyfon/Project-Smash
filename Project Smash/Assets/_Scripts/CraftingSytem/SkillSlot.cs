using PSmash.Items;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PSmash.LevelUpSystem
{
    public class SkillSlot : MonoBehaviour
    {

        [SerializeField] Skill skill = null;
        [SerializeField] Image skillSlotImage = null;
        [SerializeField] SkillSlot[] UnlockableBySkillSlots;
        [SerializeField] RequiredCraftingItems[] requiredCraftingMaterials;
        [SerializeField] UnlockableSkillPath[] availablePathsOnceUnlocked;
        //[SerializeField] Image ringImage = null;

        // Start is called before the first frame update
        void Start()
        {
            skillSlotImage.sprite = skill.sprite;
        }

        /// <summary>
        /// Method used by the Button component in the same gameObject
        /// </summary>
        public void TryToUnlockSkill()
        {
            GetComponentInParent<CraftingSystem>().TryToUnlockSkill(skill, this);
        }

        /// <summary>
        /// Inform about the options to be able to unlock
        /// </summary>
        /// <returns></returns>
        public SkillSlot[] GetSkillSlotsUnlockingOptions()
        {
            return UnlockableBySkillSlots;
        }

        /// <summary>
        /// Update the material to change between the 3 visuals aspects of the crafting system
        /// Locked, Unlockable and Unlocked
        /// </summary>
        /// <param name="material"></param>
        public void UpdateImageMaterial(Material material)
        {
            skillSlotImage.material = material;
        }

        public void UpdateSkillSlotVisualState(Material material)
        {
            skillSlotImage.material = material;
            //ringImage.enabled = isRingEnabled;
        }

        /// <summary>
        /// To update the visuals of the links between Grey and White
        /// once it sets to white the links will not go back to dark again
        /// </summary>
        /// <param name="state"></param>
        public void UpdateLinks(string state)
        {
            foreach (UnlockableSkillPath unlockableSkillPath in availablePathsOnceUnlocked)
            {
                unlockableSkillPath.link.UpdateColor(state);
            }
        }

        public bool HaveNecessaryMaterials(Inventory inventory)
        {
            //Return a true of false depending on if the player has all the required materials to unlock this skill

            //Create the dictionary where the info will be stored to substract the quantity from the Player's Materials in case it will be unlocked
            Dictionary<CraftingItem, int> craftingItems = new Dictionary<CraftingItem, int>();

            //Add the CraftingMaterialList enum to the dictionary with its requiredquantity to unlock
            //In case the player does not meet the requirements of any material it will return a false inmediately
            //In contrary case the foreach loop will end with the dictionary completed
            foreach (RequiredCraftingItems requiredItems in requiredCraftingMaterials)
            {
                //print("The required material " + requiredMaterial.material);
                int myCraftingItemNumber = inventory.GetThisCraftingItemNumber(requiredItems.item);
                if (myCraftingItemNumber >= requiredItems.number)
                {
                    craftingItems.Add(requiredItems.item, requiredItems.number);
                    continue;
                }
                else
                {
                    return false;
                }
            }

            int requiredNumberToUnlock = 0;
            //With the dictionary completed it will send the CraftingMaterialList enum with the required material 
            //to the MyCraftingMaterials to substract the required ammount from the current posesed by the player
            //print("The Player has all the required materials ");
            foreach (CraftingItem material in craftingItems.Keys)
            {
                requiredNumberToUnlock = craftingItems[material];
                inventory.UpdateThisCraftingItem(material, -requiredNumberToUnlock);
            }
            return true;
        }

        public Dictionary<CraftingItem, int> GetCraftingItemsRequirement()
        {
            Dictionary<CraftingItem, int> requiredCraftingItems = new Dictionary<CraftingItem, int>();

            foreach (RequiredCraftingItems requiredCraftingMaterial in requiredCraftingMaterials)
            {
                //print("Adding  " + requiredCraftingMaterial.material);
                requiredCraftingItems.Add(requiredCraftingMaterial.item, requiredCraftingMaterial.number);
            }
            //print(requiredCraftingItems.Count);
            //print("Got the Required Materials " + requiredMaterials);
            return requiredCraftingItems;
        }

        [System.Serializable]
        public class RequiredCraftingItems
        {
            public CraftingItem item;
            public int number;
        }

        [System.Serializable]
        public class UnlockableSkillPath
        {
            public Link link;
        }

    }

}
