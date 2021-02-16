using PSmash.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSmash.LevelUpSystem
{
    public class DescriptionWindow : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup = null;
        [SerializeField] float fadeTime = 1;
        [SerializeField] DescriptionWindowInfoHandler requiredMaterialsUpdater = null;
        GameObject previousMenuSelection;
        GameObject initialMenuSelection;
        EventSystem eventSystem;
        Coroutine coroutine;

        private void Awake()
        {
            eventSystem = FindObjectOfType<EventSystem>();
            initialMenuSelection = GetComponentInParent<CraftingSystem>().initialMenuSelection;
        }

        void OnEnable()
        {
            if (eventSystem != null)
            {
                if (previousMenuSelection != null && initialMenuSelection != previousMenuSelection)
                {
                    //print("Set Previous Selection");
                    eventSystem.SetSelectedGameObject(previousMenuSelection);
                    UpdateDescriptionWindow(previousMenuSelection);
                }

                else
                {
                    //print("Set Initial Selection");
                    if (initialMenuSelection == null)
                        Debug.LogWarning("The skillSlot selected is empty");
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
            if (skillSlotGameObject == null)
                Debug.LogWarning("The skillSlot selected is empty");
            coroutine = StartCoroutine(InfoUpdate(skillSlotGameObject));
        }

        IEnumerator InfoUpdate(GameObject skillSlotGameObject)
        {
            Fader fader = new Fader();
            yield return fader.FadeOut(canvasGroup, fadeTime);
            yield return InfoChange(skillSlotGameObject);
            yield return fader.FadeIn(canvasGroup, fadeTime);
        }

        IEnumerator InfoChange(GameObject skillSlotGameObject)
        {
            if (skillSlotGameObject == null)
                Debug.LogWarning("The skillSlot selected is empty");
            //print("Got the SkillSlot " + skillSlotGameObject.name);
            transform.position = eventSystem.currentSelectedGameObject.transform.position;
            Dictionary<CraftingMaterial, int> requiredCraftingMaterials = new Dictionary<CraftingMaterial, int>();
            requiredCraftingMaterials = skillSlotGameObject.GetComponent<SkillSlot>().GetCraftingMaterialsRequirement2();
            requiredMaterialsUpdater.SetCurrentSkillSlotMaterials(requiredCraftingMaterials);
            yield return null;
        }
    }
}
