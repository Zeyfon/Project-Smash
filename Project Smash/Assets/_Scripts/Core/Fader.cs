using System.Collections;
using UnityEngine;

namespace PSmash.Core
{
    public class Fader
    {
        public IEnumerator FadeOut(CanvasGroup canvasGroup, float time)
        {
            canvasGroup.alpha = 0;
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += 1 * (Time.deltaTime / time);
                yield return null;
            }
        }

        public IEnumerator FadeIn(CanvasGroup canvasGroup, float time)
        {
            canvasGroup.alpha = 1;
            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= 1 * (Time.deltaTime / time);
                yield return null;
            }
        }

        public IEnumerator InstantFadeOut(CanvasGroup canvasGroup)
        {
            canvasGroup.alpha = 0;
            yield return null;
        }
    }
}

