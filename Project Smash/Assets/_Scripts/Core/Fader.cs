using System.Collections;
using UnityEngine;

namespace PSmash.Core
{
    public class Fader
    {
        public IEnumerator FadeIn(CanvasGroup canvasGroup, float time)
        {
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
    }
}

