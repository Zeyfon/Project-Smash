using PSmash.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PSmash.Core;

namespace PSmash.UI.Inventories
{
    public class ItemCollecteSlotUI : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup = null;
        [SerializeField] Image image = null;
        [SerializeField] TextMeshProUGUI text = null;

        internal void Setup(CraftingItem item, int number)
        {
            canvasGroup.alpha = 0;
            image.sprite = item.GetSprite();
            text.text = item.name;
            transform.SetAsFirstSibling();
            StartCoroutine(ShowAndHide());
        }

        IEnumerator ShowAndHide()
        {
            Fader fader = new Fader();
            yield return fader.FadeIn(canvasGroup, 0.3f);
            yield return new WaitForSeconds(1.5f);
            yield return fader.FadeOut(canvasGroup, 1);
            Destroy(gameObject);
        }
    }
}

