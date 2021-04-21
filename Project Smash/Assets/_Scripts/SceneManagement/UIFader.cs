using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Core;
namespace PSmash.SceneManagement
{
    public class UIFader : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup = null;

        public IEnumerator FadeIn(float time)
        {
            //print("Fading in");
            Fader fader = new Fader();
            yield return fader.FadeIn(canvasGroup, time);
        }

        public IEnumerator FadeOut(float time)
        {
            //print("Fading Out");
            Fader fader = new Fader();
            yield return fader.FadeOut(canvasGroup, time);
        }

        public void FadeOutInmediate()
        {
            Fader fader = new Fader();
            fader.InstantFadeOut(canvasGroup);
        }
    }

}
