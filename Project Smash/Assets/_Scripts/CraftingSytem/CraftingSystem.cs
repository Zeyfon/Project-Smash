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
    /// This Class is in charge of the process of Leveling up the character through a the Crafting System
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
      

        public void SetCanUnlockSkill(bool value)
        {
            canUnlockSkill = value;
        }

        /// <summary>
        /// The process to unlock the skill
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="skillSlot"></param>
        public void UnlockSkill(SkillSlot skillSlot)
        {
            PayToUnlock(skillSlot.GetRequiredCraftingItems());
            Inventory.GetPlayerInventory().UnlockSkill(skillSlot.GetItem());
            OnSkillPanelUpdate(skillSlot);
            audioSource.PlayOneShot(skillUnlockedSound, volume);
        }

        void PayToUnlock(SkillSlot.CraftingItemSlot[] slots)
        {
            Inventory.GetPlayerInventory().SubstractTheseCraftingItemsNumbers(slots);
        }
        /////////////////////////////////////////////////////////////////SAVING SYSTEM/////////////////////////////////////////////

        public object CaptureState()
        {
            Dictionary<int, bool> state = new Dictionary<int, bool>();
            int i = 0;
            foreach(SkillSlot slot in transform.GetChild(0).GetChild(1).GetComponentsInChildren<SkillSlot>())
            {

                state.Add(i, slot.IsUnlocked());
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
