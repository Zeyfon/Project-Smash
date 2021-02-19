using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace PSmash.LevelUpSystem
{
    public class SkillPanel : MonoBehaviour
    {
        [SerializeField] Material saturationCeroMaterial = null;
        [SerializeField] Material yellowSelectorMaterial = null;
        [SerializeField] CraftingSystem craftingSystem = null;
        
        EventSystem eventSystem;

        void Awake()
        {
            eventSystem = FindObjectOfType<EventSystem>();
        }
        public void SetSkillSlotAsLocked(SkillSlot skillSlot)
        {
            //Saturation 0
            skillSlot.UpdateSkillSlotVisualState(saturationCeroMaterial);
            SetThisSkillSlotRingBasedOnState(skillSlot.gameObject);
            skillSlot.UpdateLinks("Dark");
        }

        public void SetSkillSlotAsUnlockable(SkillSlot skillSlot)
        {
            //Saturation 0 with White Ring
            skillSlot.UpdateSkillSlotVisualState(saturationCeroMaterial);
            SetThisSkillSlotRingBasedOnState(skillSlot.gameObject);

            skillSlot.UpdateLinks("Dark");
        }

        public void SetSkillSlotAsUnlocked(SkillSlot skillSlot)
        {
            //Saturation 1
            skillSlot.UpdateSkillSlotVisualState(null);
            SetThisSkillSlotRingBasesOnSelection(skillSlot.gameObject);
            skillSlot.UpdateLinks("White");
        }

        private void SetThisSkillSlotRingBasesOnSelection(GameObject gameObject)
        {
            if (gameObject == eventSystem.currentSelectedGameObject)
                SetThisSkillSlotRingToYellow(gameObject);
            else
            {
                Image skillSlotImage = gameObject.GetComponentInChildren<Image>();
                skillSlotImage.enabled = false;
            }
        }

        public void SetRingsForTheseGameObjects(GameObject currentSelection, GameObject previousSelection)
        {
            SetThisSkillSlotRingToYellow(currentSelection);
            if (previousSelection == null || currentSelection == previousSelection)
                return;
            SetThisSkillSlotRingBasedOnState(previousSelection);
        }

        private void SetThisSkillSlotRingToYellow(GameObject skillSlotSelection)
        {
            //print("Yellow ring enabled for " + skillSlotSelection.name);
            Image currentSkillSlotImage = skillSlotSelection.GetComponentInChildren<Image>();
            currentSkillSlotImage.material = yellowSelectorMaterial;
            currentSkillSlotImage.enabled = true;
        }

        void SetThisSkillSlotRingBasedOnState(GameObject skillSlotSelection)
        {
            
            Image previousSkillSlotImage = skillSlotSelection.GetComponentInChildren<Image>();
            SkillSlot skillSlot = skillSlotSelection.GetComponent<SkillSlot>();
            if (!craftingSystem.IsAnySkillSlotPathUnlocked(skillSlot) || craftingSystem.IsSkillUnlocked(skillSlot))
            {
                //print("Disabling Ring for " + skillSlotSelection.name);
                previousSkillSlotImage.enabled = false;
            }
            else
            {
                previousSkillSlotImage.enabled = true;
            }
            previousSkillSlotImage.material = null;
        }
    }
}

