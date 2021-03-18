using PSmash.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Items;
using PSmash.UI;

namespace PSmash.LevelUpSystem
{
    public class DescriptionWindow : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup = null;
        [SerializeField] float fadeTime = 1;
        [SerializeField] DescriptionWindowInfoHandler requiredMaterialsUpdater = null;

        Coroutine coroutine;
        bool isInitialized = false;
        public void UpdateDescriptionWindow(GameObject skillSlotGameObject)
        {
            //print("Wants to Update Description Window");

            if (coroutine != null)
                StopCoroutine(coroutine);
            if (skillSlotGameObject == null)
                Debug.LogWarning("The skillSlot selected is empty");

            if (isInitialized)
            {
                coroutine = StartCoroutine(InfoUpdate(skillSlotGameObject));
            }
            isInitialized = true;
        }

        IEnumerator InfoUpdate(GameObject skillSlotGameObject)
        {
            //print("Updating Description Window");
            Fader fader = new Fader();
            yield return fader.FadeOut(canvasGroup, fadeTime);
            yield return InfoChange(skillSlotGameObject);
            yield return fader.FadeIn(canvasGroup, fadeTime);
        }

        IEnumerator InfoChange(GameObject skillSlotGameObject)
        {
            if (skillSlotGameObject == null)
                Debug.LogWarning("The skillSlot selected is empty");
            transform.position = skillSlotGameObject.transform.position;
            Dictionary<CraftingItem, int> requiredCraftingMaterials;
            requiredCraftingMaterials = skillSlotGameObject.GetComponent<SkillSlot>().GetCraftingItemsRequirement();
            //print(requiredCraftingMaterials.Count);
            requiredMaterialsUpdater.SetSkillSlotInfo(requiredCraftingMaterials);
            yield return null;
        }
    }
}
