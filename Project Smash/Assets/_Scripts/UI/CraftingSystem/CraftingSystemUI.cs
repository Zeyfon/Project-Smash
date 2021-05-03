using PSmash.CraftingSystem;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSmash.UI.CraftingSytem
{
    public class CraftingSystemUI : MonoBehaviour
    {
        //CONFIG
        [SerializeField] SkillSlot initialSkillSlot = null;
        [SerializeField] Material yellowMaterial = null;

        //STATE

        public delegate void SelectionChange(SkillSlot gameObject);
        public static event SelectionChange onSelectionChange;


        EventSystem eventSystem;
        GameObject previousGameObject;

        //INITIALIZE
        private void Start()
        {
            eventSystem = FindObjectOfType<EventSystem>();
        }


        private void OnEnable()
        {
            CraftingSystem.CraftingSystem.OnSkillPanelUpdate += UpdateSkillPanel;
            UpdateSkillPanel(initialSkillSlot);
        }

        private void OnDisable()
        {
            CraftingSystem.CraftingSystem.OnSkillPanelUpdate -= UpdateSkillPanel;
        }

        ////////////////////////////////////////////////////////////////////////////////PUBLIC/////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets the SkillSlot to Unlocked, Unlockable and Locked states
        /// </summary>
        public void UpdateSkillPanel(SkillSlot currentSelectedSkillSlot)
        {
            print("Updating Panel");
            foreach (SkillSlot slot in GetComponentsInChildren<SkillSlot>())
            {
                slot.VisualUpdate();
            }
            if(currentSelectedSkillSlot == null)
            {
                UpdateRings(initialSkillSlot);
            }
            else
            {
                UpdateRings(currentSelectedSkillSlot);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////PRIVATE/////////////////////////////////////////////////////////////////////


        void Update()
        {
            if (eventSystem == null)
                return;
            if (previousGameObject != eventSystem.currentSelectedGameObject)
            {
                SkillSlot skillSlot = eventSystem.currentSelectedGameObject.GetComponent<SkillSlot>();
                if (skillSlot == null)
                {
                    skillSlot = initialSkillSlot;
                    eventSystem.SetSelectedGameObject(skillSlot.gameObject);
                }
                
                UpdateRings(skillSlot);
            }
        }


        void UpdateRings(SkillSlot skillSlot)
        {
            CraftingSystem.CraftingSystem craftingSystem = GetComponentInParent<CraftingSystem.CraftingSystem>();
            //TO DO
            foreach(SkillSlot slot in GetComponentsInChildren<SkillSlot>())
            {
                if(slot.IsUnlockable() && craftingSystem.DoIHaveTheNecessaryItemNumbersToUnlockThisSkill(slot) != null)
                {
                    slot.SetRightToWhite();
                }
                else
                {
                    slot.SetRingToNull();
                }
            }
            skillSlot.SetRingToYellow(yellowMaterial);
            //SET TO WHITE ALL THE RIGHTS WHOSE SKILLSLOT ISUNLOCKABLE AND HASALLTHECRAFTINGMATERIALSREQUIREDTOUNLOCK
            //SET TO YELLOW THE CURRENT SELECTION
            //if (previousGameObject != null && previousGameObject != skillSlot.gameObject)
            //{
            //    previousGameObject.GetComponent<SkillSlot>().SetRingMaterial(null);
            //    UpdateSkillPanel();
            //}
            
            //Update ToolTipWindow
            if (onSelectionChange != null)
            {
                onSelectionChange(skillSlot);
            }
            //skillSlot.SetRingMaterial(yellowMaterial);

            previousGameObject = skillSlot.gameObject;
        }

    }
}

