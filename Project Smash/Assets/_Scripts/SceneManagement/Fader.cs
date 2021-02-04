using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Core
{
    public class Fader : MonoBehaviour
    {
        public IEnumerator FadeIn(CanvasGroup canvasGroup, float time)
        {
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += 1 * (Time.deltaTime / time);
                yield return null;
            }
        }

        public IEnumerator FadeIn(float time)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += 1 * (Time.deltaTime / time);
                yield return null;
            }
        }

        public IEnumerator FadeOut(CanvasGroup canvasGroup, float time)
        {
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= 1 * (Time.deltaTime / time);
                yield return null;
            }
        }

        public IEnumerator FadeOut(float time)
        {
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= 1 * (Time.deltaTime / time);
                yield return null;
            }
        }
    }
}

