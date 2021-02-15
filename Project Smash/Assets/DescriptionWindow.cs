using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSmash.LevelUpSystem
{
    public class DescriptionWindow : MonoBehaviour
    {
        [SerializeField] DescriptionWindowInfoHandler requiredMaterialsUpdater = null;
        GameObject previousMenuSelection;
        GameObject initialMenuSelection;
        EventSystem eventSystem;
        Coroutine coroutine;

        private void Awake()
        {
            //initialMenuSelection = FindObjectOfType<CraftingSystem>().initialMenuSelection;
            //previousMenuSelection = initialMenuSelection;
            eventSystem = FindObjectOfType<EventSystem>();

        }

        void OnEnable()
        {
            if (eventSystem != null)
            {
                if (initialMenuSelection != null && initialMenuSelection != previousMenuSelection)
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
        }

        private void Update()
        {
            if (eventSystem == null)
                return;
            if (previousMenuSelection != eventSystem.currentSelectedGameObject)
            {
                previousMenuSelection = eventSystem.currentSelectedGameObject;
                UpdateDescriptionWindow(previousMenuSelection);
            }
        }

        private void UpdateDescriptionWindow(GameObject skillSlotGameObject)
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(InfoUpdate(skillSlotGameObject));

        }

        IEnumerator InfoUpdate(GameObject skillSlotGameObject)
        {
            yield return FadeOut();
            yield return InfoChange(skillSlotGameObject);
            yield return FadeIn();
        }

        IEnumerator FadeOut()
        {
            yield return null;
        }

        IEnumerator InfoChange(GameObject skillSlotGameObject)
        {
            print("Got the SkillSlot " + skillSlotGameObject.name);
            transform.position = eventSystem.currentSelectedGameObject.transform.position;
            //skillSlotGameObject.GetComponent<SkillSlot>().UpdateDescriptionWindow(GetComponent<UIDescriptionWindow>());
            Dictionary<CraftingMaterialsList, int> requiredMaterials = new Dictionary<CraftingMaterialsList, int>();
            requiredMaterials = skillSlotGameObject.GetComponent<SkillSlot>().GetCraftingMaterialsRequirement();
            requiredMaterialsUpdater.SetRequiredMaterials(requiredMaterials);
            yield return null;
        }

        IEnumerator FadeIn()
        {
            yield return null;
        }
    }

}
