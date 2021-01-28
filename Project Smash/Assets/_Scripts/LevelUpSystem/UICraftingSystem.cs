using PSmash.InputSystem;
using PSmash.Stats;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSmash.LevelUpSystem
{
    /// <summary>
    /// This Class is in charge of the general process of the Crafting System
    /// and the update of the corresponding variables in the BaseStats script
    /// of the player (Stats and materials-for now)
    /// </summary>
    public class UICraftingSystem : MonoBehaviour
    {
        [SerializeField] Material skillLockedMaterial = null;
        [SerializeField] Material skillUnlockableMaterial = null;
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] AudioClip skillUnlockedSound = null;
        [SerializeField] AudioClip skillCannotBeUnlockedSound = null;
        [SerializeField] float volume = 1;
        [SerializeField] GameObject initialMenuSelection = null;
        [SerializeField] UIDescriptionWindow descriptionWindow = null;

        _Controller _controller;
        GameObject previousMenuSelection;
        EventSystem eventSystem;
        BaseStats playerStats;
        List<SkillSlot> unlockedSkillSlots = new List<SkillSlot>();
        SkillSlot[] skillSlots;
        UICraftingMaterial[] uiMaterials;

        private void Awake()
        {
            previousMenuSelection = initialMenuSelection;
            _controller = new _Controller();
            eventSystem = FindObjectOfType<EventSystem>();
            playerStats = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseStats>();
        }
        // Start is called before the first frame update
        void Start()
        {
            skillSlots = transform.GetComponentsInChildren<SkillSlot>();
            uiMaterials = transform.GetComponentsInChildren<UICraftingMaterial>();
            UpdateUIVisuals();
            descriptionWindow.gameObject.SetActive(false);
            gameObject.SetActive(false);
        }

        /// <summary>
        /// The enable will enable the UI Input System and will disable the Input of the player
        /// Also will enable the crafting UI with the last selection
        /// </summary>
        void OnEnable()
        {
            if (eventSystem != null)
            {
                if (initialMenuSelection != previousMenuSelection)
                {
                    eventSystem.SetSelectedGameObject(previousMenuSelection);
                    UpdateDescriptionWindow(previousMenuSelection);
                }

                else
                {
                    eventSystem.SetSelectedGameObject(initialMenuSelection);
                    UpdateDescriptionWindow(initialMenuSelection);

                }

            }
            print("Crafting Input Enabled");
            _controller.UI.Enable();
            _controller.UI.Cancel.performed += ctx => CancelAction();
        }

        void OnDisable()
        {
            print("Craftin Input Disabled");
            _controller.UI.Disable();
            _controller.UI.Cancel.performed -= ctx => CancelAction();
        }

        void CancelAction()
        {
            print("Canceling Action");
            descriptionWindow.gameObject.SetActive(false);
            gameObject.SetActive(false);
            playerStats.transform.GetChild(0).GetComponent<InputHandler>().EnableInput(true);
        }
        /// <summary>
        /// The Update will be checking and updating the current selections in the Event System for the UI
        /// and will trigger the method of Updating the Description Window. Forcing it to update its values
        /// and to start all over the show process(Waiting Time and Fade In Processes)
        /// </summary>
        private void Update()
        {
            if (eventSystem == null)
                return;
            if (previousMenuSelection != eventSystem.currentSelectedGameObject)
            {
                //print(eventSystem.currentSelectedGameObject);
                previousMenuSelection = eventSystem.currentSelectedGameObject;
                UpdateDescriptionWindow(previousMenuSelection);
            }
        }

        private void UpdateDescriptionWindow(GameObject skillSlotGameObject)
        {
            descriptionWindow.gameObject.transform.position = eventSystem.currentSelectedGameObject.transform.position;
            skillSlotGameObject.GetComponent<SkillSlot>().UpdateDescriptionWindow(descriptionWindow);
        }

        /// <summary>
        /// This method is the one that starts and enables the Crafting System.
        /// This one is triggered by the SavingPoint once the player is inside its trigger.
        /// This method es enabled by the Button A / Space Key from the Input Handler of the player
        /// </summary>
        public void EnableMenu()
        {
            UpdateUIVisuals();
            gameObject.SetActive(true);
            descriptionWindow.gameObject.SetActive(true);
            playerStats.transform.GetChild(0).GetComponent<InputHandler>().EnableInput(false);

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
                CannotUnlockSkillEffects();
            }
            else
            {
                if (IsAnySkillSlotUnlockableOptionsUnlocked(skillSlot) && HaveNecessaryMaterials(skillSlot))
                {
                    UnlockSkill(skill, skillSlot);
                }
                else
                {
                    CannotUnlockSkillEffects();
                }
            }
        }

        bool IsSkillUnlocked(SkillSlot skillSlot)
        {
            if (unlockedSkillSlots.Contains(skillSlot))
                return true;
            else
                return false;
        }

        private bool IsAnySkillSlotUnlockableOptionsUnlocked(SkillSlot skillSlot)
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
            return skillSlot.HaveNecessaryMaterials(playerStats);
        }

        private void CannotUnlockSkillEffects()
        {
            audioSource.PlayOneShot(skillCannotBeUnlockedSound, volume);
        }

        private void UnlockSkill(Skill skill, SkillSlot skillSlot)
        {
            unlockedSkillSlots.Add(skillSlot);
            playerStats.UnlockSkill(skill);
            UpdateUIVisuals();
            audioSource.PlayOneShot(skillUnlockedSound, volume);
        }

        /// <summary>
        /// If a skill is  unlocked the visuals of the Crafting System UI will be updated all over again
        /// making the necessary changes to the crafting materials in the main window, skillSlots and links to reflex the new options
        /// </summary>
        void UpdateUIVisuals()
        {
            //Update the material in each skillSlot
            foreach (SkillSlot skillSlot in skillSlots)
            {
                if (IsSkillUnlocked(skillSlot))
                {
                    skillSlot.UpdateImageMaterial(null);
                    skillSlot.UpdateLinks("White"); ;

                }
                else
                {
                    if (IsAnySkillSlotUnlockableOptionsUnlocked(skillSlot))
                    {
                        skillSlot.UpdateImageMaterial(skillUnlockableMaterial);
                        skillSlot.UpdateLinks("Dark"); ;
                    }
                    else
                    {
                        skillSlot.UpdateImageMaterial(skillLockedMaterial);
                        skillSlot.UpdateLinks("Dark"); ;

                    }
                }
            }

            //Update Values for the Crafting Materials owner showd in the UI Crafting System
            foreach (UICraftingMaterial uiMaterial in uiMaterials)
            {
                int value = playerStats.GetMaterialQuantity(uiMaterial.material);
                //print(uiMaterial.material + "  "  + value);
                uiMaterial.UpdateValue(value);
            }
        }
    }
}
