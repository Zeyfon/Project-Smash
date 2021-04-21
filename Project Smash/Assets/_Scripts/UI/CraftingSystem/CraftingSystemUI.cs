using PSmash.CraftingSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PSmash.UI.CraftingSytem
{
    public class CraftingSystemUI : MonoBehaviour
    {
        [SerializeField] SkillSlot initialSkillSlot = null;
        [SerializeField] Material yellowMaterial = null;


        public delegate void SelectionChange(SkillSlot gameObject);
        public static event SelectionChange onSelectionChange;


        EventSystem eventSystem;
        GameObject previousGameObject;

        private void Start()
        {
            eventSystem = FindObjectOfType<EventSystem>();
        }

        private void Update()
        {
            if (eventSystem == null)
                return;
            if (previousGameObject != eventSystem.currentSelectedGameObject)
            {
                SkillSlot skillSlot = eventSystem.currentSelectedGameObject.GetComponent<SkillSlot>();
                if(skillSlot == null)
                {
                    skillSlot = initialSkillSlot;
                    eventSystem.SetSelectedGameObject(skillSlot.gameObject);
                }
                UpdateRing(skillSlot);
            }
        }

        private void OnEnable()
        {
            CraftingSystem.CraftingSystem.OnSkillPanelUpdate += UpdateSkillPanel;
            UpdateRing(initialSkillSlot);
        }

        private void OnDisable()
        {
            CraftingSystem.CraftingSystem.OnSkillPanelUpdate -= UpdateSkillPanel;
        }


        void UpdateRing(SkillSlot skillSlot)
        {
            if (previousGameObject != null && previousGameObject != skillSlot.gameObject)
            {
                previousGameObject.GetComponent<SkillSlot>().SetRingMaterial(null);
                UpdateSkillPanel();

            }
            if (onSelectionChange != null)
            {
                onSelectionChange(skillSlot);
            }
            skillSlot.SetRingMaterial(yellowMaterial);

            previousGameObject = skillSlot.gameObject;
        }

        public void UpdateSkillPanel()
        {
            print("Updating Panel");
            foreach(SkillSlot slot in GetComponentsInChildren<SkillSlot>())
            {
                slot.VisualUpdate();
            }
        }
    }
}

