using UnityEngine;
using UnityEngine.EventSystems;

namespace PSmash.LevelUpSystem
{
    public class CraftingSystemEventSystemSelector : MonoBehaviour
    {

        [SerializeField] DescriptionWindow descriptionWindow = null;
        [SerializeField] SkillPanel skillPanel = null;
        [SerializeField] GameObject currentSkillSelected = null;

        EventSystem eventSystem;
        GameObject previousSkillSelected;
        // Start is called before the first frame update
        void Awake()
        {
            eventSystem = FindObjectOfType<EventSystem>();           
        }

        void OnEnable()
        {
            if (eventSystem != null)
            {
                //print(gameObject.name + " enabled");
                if (previousSkillSelected != null && currentSkillSelected != previousSkillSelected)
                {
                    //print("Set Previous Selection");
                    eventSystem.SetSelectedGameObject(previousSkillSelected);
                    descriptionWindow.UpdateDescriptionWindow(previousSkillSelected);
                    skillPanel.SetRingsForTheseGameObjects(previousSkillSelected, null);

                }
                else
                {
                    //print("Set Current Selection");
                    if (currentSkillSelected == null)
                        Debug.LogWarning("The skillSlot selected is empty");
                    eventSystem.SetSelectedGameObject(currentSkillSelected);
                    descriptionWindow.UpdateDescriptionWindow(currentSkillSelected);
                    skillPanel.SetRingsForTheseGameObjects(currentSkillSelected, previousSkillSelected);
                }
            }
        }

        private void Update()
        {
            if (eventSystem == null)
                return;
            if (previousSkillSelected != eventSystem.currentSelectedGameObject)
            {
                //print("EventSystem selection changed");
                descriptionWindow.UpdateDescriptionWindow(eventSystem.currentSelectedGameObject);
                skillPanel.SetRingsForTheseGameObjects(eventSystem.currentSelectedGameObject, previousSkillSelected);
                previousSkillSelected = eventSystem.currentSelectedGameObject;
            }
        }
    }

}


