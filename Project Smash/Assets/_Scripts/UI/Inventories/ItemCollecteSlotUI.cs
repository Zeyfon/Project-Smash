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
        [SerializeField] TextMeshProUGUI nameText = null;
        [SerializeField] TextMeshProUGUI numberText = null;

        internal void Setup(Pickup.ItemSlot slot)
        {
            canvasGroup.alpha = 0;
            image.sprite = slot.item.GetSprite();
            nameText.text = slot.item.GetDisplayName();
            numberText.text = slot.number.ToString();
            transform.SetAsFirstSibling();
            StartCoroutine(ShowAndHide());
        }

        IEnumerator ShowAndHide()
        {
            Fader fader = new Fader();
            yield return fader.FadeOut(canvasGroup, 0.3f);
            yield return new WaitForSeconds(1.5f);
            yield return fader.FadeIn(canvasGroup, 1);
            Destroy(gameObject);
        }
    }
}

