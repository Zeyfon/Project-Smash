using PSmash.Inventories;
using PSmash.Menus;
using System;
using System.Collections.Generic;
using UnityEngine;
using PSmash.SceneManagement;
using GameDevTV.Saving;

namespace PSmash.CraftingSystem
{
    /// <summary>
    /// This Class is in charge of the general process of the Crafting System
    /// and the update of the corresponding variables in the BaseStats script
    /// of the player (Stats and materials-for now)
    /// </summary>
    public class CraftingSystem : MonoBehaviour, ISaveable
    {
        //CONFIG
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] AudioClip skillUnlockedSound = null;
        [SerializeField] AudioClip skillCannotBeUnlockedSound = null;
        [SerializeField] float volume = 1;

        //STATE
        public static event Action OnMenuClose;
        public delegate void SkillPanelUpdate(SkillSlot slot);
        public static event SkillPanelUpdate OnSkillPanelUpdate;

        bool canUnlockSkill = false;

        /////////////////////////////////////////////////////////////////PUBLIC////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// This method is triggered by the SkillSlot once is being presses in the UI.
        /// This method will try to unlock the skill, but will check first if it has already been unlodked
        /// if it has a connected skillSlot unlocked so this one can be unlocked ( Requirement to be like a Skill Tree)
        /// if it has the required materials to be able to unlock
        /// if all previous requirements are fulfilled the skill will be unlocked
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="skillSlot"></param>
        public void TryToUnlockSkill(SkillSlotButton skillSlotButton)
        {
            SkillSlot skillSlot = skillSlotButton.GetComponent<SkillSlot>();
            //print(skill.name + "  " + skillSlot.gameObject.name);
            if (IsSkillUnlocked(skillSlot))
            {
                CannotUnlockSkill();
            }
            else
            {
                Dictionary<CraftingItem, int> craftingItemsRequired = DoPlayerHasTheNecessaryItemNumbersToUnlockThisSkill(skillSlot);
                   if (IsThisSkillSlotAvailableToUnlock(skillSlot) && craftingItemsRequired != null && GetCanUnlockSkill())
                {
                    SubstractItemsFromInventory(craftingItemsRequired);
                    UnlockSkill(skillSlot);
                    OnSkillPanelUpdate(skillSlot);
                }
                else
                {
                    CannotUnlockSkill();
                }
            }
        }

        /// <summary>
        /// Returns all the craftingMaterial necessary to unlock this skill only if you have all the necessary materials in your inventory
        /// </summary>
        /// <param name="skillSlot"></param>
        /// <returns></returns>
        public Dictionary<CraftingItem, int> DoPlayerHasTheNecessaryItemNumbersToUnlockThisSkill(SkillSlot skillSlot)
        {
            SkillSlot.CraftingItemSlot[] slots = skillSlot.GetRequiredCraftingItems();
            Dictionary<CraftingItem, int> itemsNeeded = new Dictionary<CraftingItem, int>();

            foreach (SkillSlot.CraftingItemSlot slot in slots)
            {
                int numberNeeded = Inventory.GetPlayerInventory().GetThisCraftingItemNumber(slot.item);
                if (numberNeeded >= slot.number)
                {
                    itemsNeeded.Add(slot.item, slot.number);
                    continue;
                }
                else
                {
                    //print("Items Criteria is not fulfilled to unlock skill");
                    return null;
                }
            }
            //print("Items criteria is fullfilled to unlock skill");
            return itemsNeeded;
        }

        public void UpdatePanel(SkillSlot slot)
        {
            OnSkillPanelUpdate(slot);
        }



        ///////////////////////////////////////////////////////////////PRIVATE//////////////////////////////////////////////////////////////////
        /// <summary>
        /// Checks if the skillSlot has already been unlocked
        /// </summary>
        /// <param name="skillSlot"></param>
        /// <returns></returns>
        bool IsSkillUnlocked(SkillSlot skillSlot)
        {
            if (skillSlot.GetIsUnlocked())
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks if the skill tree has reached this skillslot.
        /// </summary>
        /// <param name="skillSlot"></param>
        /// <returns></returns>
        bool IsThisSkillSlotAvailableToUnlock(SkillSlot skillSlot)
        {
            SkillSlot[] skillSlotsUnlockingOptions = skillSlot.GetSkillSlotsUnlockingOptions();
            //print(skillSlot.gameObject.name + " needs that the " + tempSkillSlot + "  is unlocked to unlock ");
            if (skillSlotsUnlockingOptions.Length == 0)
                return true;
            else
            {
                foreach (SkillSlot unlockableSkillSlotOption in skillSlotsUnlockingOptions)
                {
                    if (unlockableSkillSlotOption.GetIsUnlocked())
                    {
                        print(skillSlot.name + "  can be unlock. There is an unlocked option");
                        return true;
                    }
                }
                //print(skillSlot.name +  "  cannot be unlocked. There is no unlocked option");
                return false;
            }
        }

        bool GetCanUnlockSkill()
        {
            return canUnlockSkill;
        }

        public void SetCanUnlockSkill(bool value)
        {
            canUnlockSkill = value;
        }

        /// <summary>
        /// Effects telling you that this skill cannot be unlocked.
        /// </summary>
        void CannotUnlockSkill()
        {
            audioSource.PlayOneShot(skillCannotBeUnlockedSound, volume);
        }

        /// <summary>
        /// The process to unlock the skill
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="skillSlot"></param>
        void UnlockSkill(SkillSlot skillSlot)
        {
            Inventory.GetPlayerInventory().UnlockSkill(skillSlot.GetItem());
            skillSlot.SetIsUnlocked(true);
            audioSource.PlayOneShot(skillUnlockedSound, volume);
        }

        void SubstractItemsFromInventory(Dictionary<CraftingItem,int> craftingItemsRequired)
        {
            Inventory.GetPlayerInventory().SubstractTheseCraftingItemsNumbers(craftingItemsRequired);
        }
        /////////////////////////////////////////////////////////////////SAVING SYSTEM/////////////////////////////////////////////

        public object CaptureState()
        {
            Dictionary<int, bool> state = new Dictionary<int, bool>();
            int i = 0;
            foreach(SkillSlot slot in transform.GetChild(0).GetChild(1).GetComponentsInChildren<SkillSlot>())
            {

                state.Add(i, slot.GetIsUnlocked());
                    i++;
            }
            return state;
        }

        public void RestoreState(object state, bool isLoadLastScene)
        {

            Dictionary<int, bool> obj = (Dictionary<int, bool>)state;
            SkillSlot[] slots = transform.GetChild(0).GetChild(1).GetComponentsInChildren<SkillSlot>();
            for( int i = 0; i<slots.Length; i++)
            {
                slots[i].SetIsUnlocked(obj[i]);
            }
        }
    }
}
