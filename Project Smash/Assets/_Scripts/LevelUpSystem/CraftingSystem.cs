using PSmash.InputSystem;
using PSmash.Stats;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.LevelUpSystem
{
    /// <summary>
    /// This Class is in charge of the general process of the Crafting System
    /// and the update of the corresponding variables in the BaseStats script
    /// of the player (Stats and materials-for now)
    /// </summary>
    public class CraftingSystem : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] AudioClip skillUnlockedSound = null;
        [SerializeField] AudioClip skillCannotBeUnlockedSound = null;
        [SerializeField] float volume = 1;
        [SerializeField] SkillPanel skillPanel = null;

        _Controller _controller;
        BaseStats playerStats;
        MyCraftingMaterials playerMaterials;

        List<SkillSlot> unlockedSkillSlots = new List<SkillSlot>();
        SkillSlot[] skillSlots;

        private void Awake()
        {
            _controller = FindObjectOfType<InputHandler>().GetController();

            //_controller = new _Controller();
            playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
            playerMaterials = GameObject.FindGameObjectWithTag("Player").GetComponent<MyCraftingMaterials>();

            skillSlots = transform.GetComponentsInChildren<SkillSlot>();
        }

        // Start is called before the first frame update
        void Start()
        {
            UpdateSkillPanel();
            EnableCraftingSystem(false);
        }

        void EnableCraftingSystem(bool state)
        {
            for(int i = 0; i<transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(state);
            }
        }

        void CancelAction()
        {
            print("Canceling Action");
            EnableCraftingSystem(false);
            playerStats.transform.GetChild(0).GetComponent<InputHandler>().EnableInput(true);

            _controller.UI.Cancel.performed -= ctx => CancelAction();
            _controller.UI.Disable();
        }

        /// <summary>
        /// This method is the one that starts and enables the Crafting System.
        /// This one is triggered by the SavingPoint once the player is inside its trigger.
        /// This method es enabled by the Button A / Space Key from the Input Handler of the player
        /// </summary>
        public void EnableMenu()
        {
            UpdateSkillPanel();
            EnableCraftingSystem(true);
            playerStats.transform.GetChild(0).GetComponent<InputHandler>().EnableInput(false);
            _controller.UI.Enable();
            _controller.UI.Cancel.performed += ctx => CancelAction();
        }

        /// <summary>
        /// This method is triggered by the SkillSlot once is being presses in the UI.
        /// This method will try to unlock the skill, but will check first if it has already been unlodked
        /// if it has a connected skillSlot unlocked so this one can be unlocked ( Requirement to be like a Skill Tree)
        /// if it has the required materials to be able to unlock
        /// if all previous requirements are fulfilled the skill will be unlocked
        /// </summary>
        /// <param name="skill"></param>
        /// <param name="skillSlot"></param>
        public void TryToUnlockSkill(Skill skill, SkillSlot skillSlot)
        {
            //print(skill.name + "  " + skillSlot.gameObject.name);
            if (IsSkillUnlocked(skillSlot))
            {
                CannotUnlockSkill();
            }
            else
            {
                if (IsAnySkillSlotPathUnlocked(skillSlot) && HaveNecessaryMaterials(skillSlot))
                {
                    UnlockSkill(skill, skillSlot);
                }
                else
                {
                    CannotUnlockSkill();
                }
            }
        }

        public bool IsSkillUnlocked(SkillSlot skillSlot)
        {
            if (unlockedSkillSlots.Contains(skillSlot))
                return true;
            else
                return false;
        }

        public bool IsAnySkillSlotPathUnlocked(SkillSlot skillSlot)
        {
            SkillSlot[] skillSlotsUnlockingOptions = skillSlot.GetSkillSlotsUnlockingOptions();
            //print(skillSlot.gameObject.name + " needs that the " + tempSkillSlot + "  is unlocked to unlock ");
            if (skillSlotsUnlockingOptions.Length == 0)
                return true;
            else
            {
                bool isUnlockableOptionsUnlocked = false;
                foreach (SkillSlot unlockableSkillSlotOptions in skillSlotsUnlockingOptions)
                {
                    if (unlockedSkillSlots.Contains(unlockableSkillSlotOptions))
                    {
                        isUnlockableOptionsUnlocked = true;
                        break;
                    }
                }
                if (isUnlockableOptionsUnlocked)
                    return true;
                else
                    return false;
            }
        }

        private bool HaveNecessaryMaterials(SkillSlot skillSlot)
        {
            //print("Checking if having necessary materials in the Crafting System ");
            return skillSlot.HaveNecessaryMaterials(playerMaterials);
        }

        private void CannotUnlockSkill()
        {
            audioSource.PlayOneShot(skillCannotBeUnlockedSound, volume);
        }

        private void UnlockSkill(Skill skill, SkillSlot skillSlot)
        {
            unlockedSkillSlots.Add(skillSlot);
            playerStats.UnlockSkill(skill);
            UpdateSkillPanel();
            audioSource.PlayOneShot(skillUnlockedSound, volume);
            //UpdatePlayerMaterialsInDescriptionWindow());
        }


        /// <summary>
        /// If a skill is  unlocked the visuals of the Crafting System UI will be updated all over again
        /// making the necessary changes to the crafting materials in the main window, skillSlots and links to reflex the new options
        /// </summary>
        void UpdateSkillPanel()
        {
            //Update the material in each skillSlot
            foreach (SkillSlot skillSlot in skillSlots)
            {
                if (IsSkillUnlocked(skillSlot))
                {
                    skillPanel.SetSkillSlotAsUnlocked(skillSlot);
                }
                else
                {
                    if (IsAnySkillSlotPathUnlocked(skillSlot))
                    {
                        skillPanel.SetSkillSlotAsUnlockable(skillSlot);
                    }
                    else
                    {
                        skillPanel.SetSkillSlotAsLocked(skillSlot);
                    }
                }
            }
        }
    }
}
