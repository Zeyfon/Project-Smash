using PSmash.CraftingSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using PSmash.Menus;

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
            MainMenuSelector.OnSelectionChange += CraftingSystemSubMenuSelectionChanged;
            UpdateSkillPanel(initialSkillSlot);
            GetComponentInChildren<CraftingSystemInfoPanel>().UpdateInfoPanelWithSkillInfo(initialSkillSlot);

        }

        /// <summary>
        /// Called only when the selection has changed and the crafting sub menu is active
        /// </summary>
        /// <param name="gameObject"></param>
        void CraftingSystemSubMenuSelectionChanged(GameObject gameObject)
        {
            if (eventSystem == null)
                return;
            UpdateSkillPanel(null);
            if (previousGameObject != eventSystem.currentSelectedGameObject)
            {
                SkillSlot skillSlot = eventSystem.currentSelectedGameObject.GetComponent<SkillSlot>();
                if (skillSlot == null)
                    return;

                skillSlot.SetRingToYellow(yellowMaterial);
                GetComponentInChildren<CraftingSystemInfoPanel>().UpdateInfoPanelWithSkillInfo(skillSlot);
            }
            previousGameObject = eventSystem.currentSelectedGameObject;
        }

        private void OnDisable()
        {
            CraftingSystem.CraftingSystem.OnSkillPanelUpdate -= UpdateSkillPanel;
            MainMenuSelector.OnSelectionChange -= CraftingSystemSubMenuSelectionChanged;

        }

        ////////////////////////////////////////////////////////////////////////////////PUBLIC/////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Sets the SkillSlot to Unlocked, Unlockable and Locked states
        /// </summary>
        public void UpdateSkillPanel(SkillSlot currentSelectedSkillSlot)
        {
            UpdateColorOfLinks();
            UpdateWhiteRings();
        }

        private void UpdateColorOfLinks()
        {
            foreach (SkillSlot slot in GetComponentsInChildren<SkillSlot>())
            {
                slot.VisualUpdate();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////PRIVATE/////////////////////////////////////////////////////////////////////


        void UpdateWhiteRings()
        {
            CraftingSystem.CraftingSystem craftingSystem = GetComponentInParent<CraftingSystem.CraftingSystem>();
            foreach(SkillSlot slot in GetComponentsInChildren<SkillSlot>())
            {
                if(slot.IsUnlockable() && craftingSystem.DoPlayerHasTheNecessaryItemNumbersToUnlockThisSkill(slot) != null)
                {
                    slot.SetRightToWhite();
                }
                else
                {
                    slot.SetRingToNull();
                }
            }
        }
    }
}

