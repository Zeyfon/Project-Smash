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
            MainMenuSelector.OnSelectionChange += CraftingSystemSubMenuSelectionChanged;
            GetComponentInChildren<CraftingSystemInfoPanel>().UpdateInfoPanelWithSkillInfo(initialSkillSlot);

        }

        private void OnDisable()
        {
            MainMenuSelector.OnSelectionChange -= CraftingSystemSubMenuSelectionChanged;

        }

        /// <summary>
        /// Called only when the selection has changed and the crafting sub menu is active
        /// </summary>
        /// <param name="gameObject"></param>
        void CraftingSystemSubMenuSelectionChanged(GameObject gameObject)
        {
            if (eventSystem == null)
                return;
            if (previousGameObject != eventSystem.currentSelectedGameObject && previousGameObject != null)
            {
                SkillSlot skillSlot = eventSystem.currentSelectedGameObject.GetComponent<SkillSlot>();
                if (skillSlot == null)
                {
                    return;
                }

                print(previousGameObject.name + eventSystem.currentSelectedGameObject.name);
                GetComponentInChildren<CraftingSystemInfoPanel>().UpdateInfoPanelWithSkillInfo(skillSlot);
                if (previousGameObject.GetComponent<SkillSlotUI>())
                {
                    previousGameObject.GetComponent<SkillSlotUI>().SetRingToWhatIsNecessary();
                }
                eventSystem.currentSelectedGameObject.GetComponent<SkillSlotUI>().SetRingToYellow();

            }
            previousGameObject = eventSystem.currentSelectedGameObject;
            print(previousGameObject.name);
        }
    }
}

