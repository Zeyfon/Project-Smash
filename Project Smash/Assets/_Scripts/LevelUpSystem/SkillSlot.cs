using PSmash.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace PSmash.LevelUpSystem
{
    public class SkillSlot : MonoBehaviour
    {

        [SerializeField] Skill skill = null;
        [SerializeField] Image skillSlotImage = null;
        [SerializeField] SkillSlot[] UnlockableBySkillSlots;
        [SerializeField] RequiredCraftingMaterial[] requiredCraftingMaterials;
        [SerializeField] UnlockableSkillPath[] availablePathsOnceUnlocked;
        [SerializeField] Image ringImage = null;

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

        public void UpdateSkillSlotVisualState(Material material, bool isRingEnabled)
        {
            skillSlotImage.material = material;
            ringImage.enabled = isRingEnabled;

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

        public bool HaveNecessaryMaterials(MyCraftingMaterials playerMaterials)
        {
            //Return a true of false depending on if the player has all the required materials to unlock this skill

            //Create the dictionary where the info will be stored to substract the quantity from the Player's Materials in case it will be unlocked
            Dictionary<CraftingMaterialsList, int> craftingMaterials = new Dictionary<CraftingMaterialsList, int>();

            //Add the CraftingMaterialList enum to the dictionary with its requiredquantity to unlock
            //In case the player does not meet the requirements of any material it will return a false inmediately
            //In contrary case the foreach loop will end with the dictionary completed
            foreach (RequiredCraftingMaterial requiredMaterial in requiredCraftingMaterials)
            {
                //print("The required material " + requiredMaterial.material);
                int quantityInPlayersPossession = playerMaterials.GetPlayerQuantityForThisMaterial(requiredMaterial.material);
                if (quantityInPlayersPossession >= requiredMaterial.quantity)
                {
                    craftingMaterials.Add(requiredMaterial.material.material, requiredMaterial.quantity);
                    continue;
                }
                else
                {
                    return false;
                }
            }

            //With the dictionary completed it will send the CraftingMaterialList enum with the required material 
            //to the MyCraftingMaterials to substract the required ammount from the current posesed by the player
            //print("The Player has all the required materials ");
            foreach (CraftingMaterialsList material in craftingMaterials.Keys)
            {
                playerMaterials.UpdateMyMaterials(material, -craftingMaterials[material]);
            }
            return true;
        }

        public Dictionary<CraftingMaterial, int> GetCraftingMaterialsRequirement2()
        {
            Dictionary<CraftingMaterial, int> requiredMaterials = new Dictionary<CraftingMaterial, int>();

            foreach (RequiredCraftingMaterial requiredCraftingMaterial in requiredCraftingMaterials)
            {
                //print("Adding  " + requiredCraftingMaterial.material);
                requiredMaterials.Add(requiredCraftingMaterial.material, requiredCraftingMaterial.quantity);
            }
            //print("Got the Required Materials " + requiredMaterials);
            return requiredMaterials;
        }

        [System.Serializable]
        public class RequiredCraftingMaterial
        {
            public CraftingMaterial material;
            public int quantity;
        }

        [System.Serializable]
        public class UnlockableSkillPath
        {
            public Link link;
        }

    }

}
