using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PSmash.UI;

namespace PSmash.LevelUpSystem
{

    public class UIDescriptionWindow : MonoBehaviour
    {
        public TextMeshProUGUI text = null;
        [SerializeField] CanvasGroup canvasGroup = null;
        [SerializeField] float timeToAppear = 5f;
        [SerializeField] float fadeInTime = 0.5f;
        public CraftingItemSlotsUI[] craftinMaterials;
        /// <summary>
        /// Here are the methods used by the current skillSlot selected
        /// to show his information (Description, and material requirements) 
        /// to be unlocked
        /// </summary>
        // Start is called before the first frame update
        void Awake()
        {
            craftinMaterials = GetComponentsInChildren<CraftingItemSlotsUI>();
        }

        public void SetWindowAlphaToCero()
        {
            canvasGroup.alpha = 0;
        }

        public IEnumerator WaitingToAppear()
        {
            //print("Waiting time started");
            float time = 0;
            while (time < timeToAppear)
            {
                //print(time);
                time += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }

        public IEnumerator FadeIn()
        {
        //print("Fading in DescriptionWindow");
            float alpha = 0;
            while (alpha < 1)
            {
                alpha += Time.deltaTime / fadeInTime;
                if (alpha > 1)
                    alpha = 1;
                canvasGroup.alpha = alpha;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}
