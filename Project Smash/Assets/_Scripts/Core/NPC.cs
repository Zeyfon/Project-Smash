using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PSmash.Core;


namespace PSmash.Core
{
    public class NPC : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        [SerializeField] float fadeTime = 1;

        Coroutine coroutine;

        private void Awake()
        {
            canvasGroup.alpha = 0;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);

               coroutine = StartCoroutine(ShowDialogue());
            }
        }

        IEnumerator ShowDialogue()
        {
            Fader fader = new Fader();
            yield return fader.FadeIn(canvasGroup, 1);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (coroutine != null)
                    StopCoroutine(coroutine);
                coroutine = StartCoroutine(HideDialogue());
            }
        }

        IEnumerator HideDialogue()
        {
            Fader fader = new Fader();
            yield return fader.FadeOut(canvasGroup, 1);
        }
    }

}
