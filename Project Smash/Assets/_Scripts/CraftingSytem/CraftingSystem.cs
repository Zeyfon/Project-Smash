using PSmash.Inventories;
using PSmash.Menus;
using System;
using System.Collections.Generic;
using UnityEngine;
using PSmash.SceneManagement;
using GameDevTV.Saving;
using PSmash.UI.CraftingSytem;

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

        Inventory inventory;
        _Controller _controller;
        TentMenu tentMenu;

        //Create the dictionary where the info will be stored to substract the quantity from the Player's Materials in case it will be unlocked
        //Dictionary<CraftingItem, int> requiredCraftingItems = new Dictionary<CraftingItem, int>();

        //INITIALIZE
        private void Awake()
        {
            _controller = new _Controller();
            inventory = Inventory.GetPlayerInventory();
            //TODO the unlocked skill slots will need to be reasigned when the save system begins to run.
        }

        // Start is called before the first frame update
        void Start()
        {
            tentMenu = FindObjectOfType<TentMenu>();
            if (tentMenu == null)
                Debug.LogWarning("No Tent Menu Found");
            //OnSkillPanelUpdate();
            CloseMenu();
        }

        ///////////////////////////////////////MENU MANAGEMENENT///////////////////

        private void OnEnable()
        {
            TentMenu.OnNextMenuOpen += OpenMenu;
        }

        private void OnDisable()
        {
            TentMenu.OnNextMenuOpen -= OpenMenu;
        }

        void OpenMenu()
        {
            SetChildObjects(true);
            OnSkillPanelUpdate(null);
            _controller.UI.Enable();
            _controller.UI.Cancel.performed += ctx => BacktrackMenu();
            _controller.UI.ButtonStart.performed += ctx => CloseAllMenus();
        }

        private void CloseAllMenus()
        {
            if(OnMenuClose != null)
            {
                OnMenuClose();
            }
            CloseMenu();
            FindObjectOfType<SavingWrapper>().Save();
            _controller.UI.Cancel.performed -= ctx => BacktrackMenu();
            _controller.UI.ButtonStart.performed -= ctx => CloseAllMenus();
            _controller.UI.Disable();
        }

        void BacktrackMenu()
        {
            //print("Backtracking Menu ");
            CloseMenu();
            FindObjectOfType<SavingWrapper>().Save();
            tentMenu.OpenTentMenuAndDoCheckpoint();
            _controller.UI.Cancel.performed -= ctx => BacktrackMenu();
            _controller.UI.ButtonStart.performed -= ctx => CloseAllMenus();
            _controller.UI.Disable();
        }

        void CloseMenu()
        {
            SetChildObjects(false);
        }

        void SetChildObjects(bool isEnabled)
        {
            for(int i = 0; i<transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(isEnabled);
            }
        }



        ////////////////////////////////////////////////////////////////////////////////////////////////////CRAFTING SYSTEM//////////////////////////////////////////////////
        

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
        public void TryToUnlockSkill(SkillSlot skillSlot)
        {
            //print(skill.name + "  " + skillSlot.gameObject.name);
            if (IsSkillUnlocked(skillSlot))
            {
                CannotUnlockSkill();
            }
            else
            {
                Dictionary<CraftingItem, int> craftingItemsRequired = DoIHaveTheNecessaryItemNumbersToUnlockThisSkill(skillSlot);
                if (IsThisSkillSlotAvailableToUnlock(skillSlot) && craftingItemsRequired != null)
                {
                    UnlockSkill(skillSlot, craftingItemsRequired);
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
        public Dictionary<CraftingItem, int> DoIHaveTheNecessaryItemNumbersToUnlockThisSkill(SkillSlot skillSlot)
        {
            SkillSlot.CraftingItemSlot[] slots = skillSlot.GetRequiredCraftingItems();
            Dictionary<CraftingItem, int> itemsNeeded = new Dictionary<CraftingItem, int>();

            foreach (SkillSlot.CraftingItemSlot slot in slots)
            {
                //print(itemRequirement.item + "  " + inventory);
                int numberNeeded = inventory.GetThisCraftingItemNumber(slot.item);
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
        void UnlockSkill(SkillSlot skillSlot, Dictionary<CraftingItem,int> craftingItemsRequired)
        {
            print(inventory);
            inventory.UnlockSkill(skillSlot.GetItem());
            SubstractItemsFromInventory(craftingItemsRequired);
            skillSlot.SetIsUnlocked(true);
            audioSource.PlayOneShot(skillUnlockedSound, volume);
            OnSkillPanelUpdate(skillSlot);
        }

        void SubstractItemsFromInventory(Dictionary<CraftingItem,int> craftingItemsRequired)
        {
            int requiredNumberToUnlock;

            //With the dictionary completed it will send the CraftingMaterialList enum with the required material 
            //to the MyCraftingMaterials to substract the required ammount from the current posesed by the player
            foreach (CraftingItem item in craftingItemsRequired.Keys)
            {
                requiredNumberToUnlock = craftingItemsRequired[item];
                inventory.UpdateThisCraftingItem(item, requiredNumberToUnlock);
            }
        }

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
            //print(slots.Length);
            for( int i = 0; i<slots.Length; i++)
            {
                slots[i].SetIsUnlocked(obj[i]);
                if(obj[i] == true)
                {
                    //print(slots[i].gameObject.name + "  is already unlocked");
                }
            }

        }
    }
}
