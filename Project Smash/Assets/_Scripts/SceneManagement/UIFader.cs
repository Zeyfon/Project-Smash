using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Core;
namespace PSmash.SceneManagement
{
    public class UIFader : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup = null;

        public IEnumerator FadeOut(float time)
        {
            Fader fader = new Fader();
            yield return fader.FadeOut(canvasGroup, time);
        }

        public IEnumerator FadeIn(float time)
        {
            Fader fader = new Fader();
            yield return fader.FadeIn(canvasGroup, time);
        }

        public void FadeOutInmediate()
        {
            Fader fader = new Fader();
            fader.InstantFadeOut(canvasGroup);
        }
    }

}
