using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PSmash.UI
{
    public class Currency : MonoBehaviour
    {
        [SerializeField] float timeToFadeOut = 2;
        [SerializeField] Text currentCurrencyText = null;
        int currentQuantity = 0;
        CanvasGroup canvasGroup;
        Coroutine coroutine;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }


        private void OnEnable()
        {
            PSmash.Items.Currency.OnCurrencyCollected += CurrencyCollected;
        }

        private void OnDisable()
        {
            PSmash.Items.Currency.OnCurrencyCollected -= CurrencyCollected;

        }

        private void CurrencyCollected()
        {
            currentQuantity++;
            currentCurrencyText.text = currentQuantity.ToString();
            canvasGroup.alpha = 1;
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(FadeOut());
        }

        IEnumerator FadeOut()
        {
            print("Starting Fade Out");
            float alpha = canvasGroup.alpha;
            yield return new WaitForSeconds(1);
            while (alpha > 0)
            {
                alpha -= Time.deltaTime / timeToFadeOut;
                if (alpha < 0) alpha = 0;
                canvasGroup.alpha = alpha;
                yield return new WaitForEndOfFrame();
            }
            print("Ending Fade Out");
            coroutine = null;
        }
    }
}

