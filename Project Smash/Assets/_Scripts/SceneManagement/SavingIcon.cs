using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PSmash.Attributes;
using PSmash.Core;

namespace PSmash.SceneManagement
{
    public class SavingIcon : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup;
        Coroutine coroutine;
        public void ShowIcon()
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(ShowingIcon());
        }
        public void HideIcon()
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(HidingIcon());
        }

        IEnumerator ShowingIcon()
        {
            print("Showing Saving Icon");
            Fader fader = new Fader();
            yield return fader.FadeIn(canvasGroup, 0.5f);
        }

        IEnumerator HidingIcon()
        {
            print("Hiding Saving Icon");
            Fader fader = new Fader();
            yield return fader.FadeOut(canvasGroup, 2);
        }
    }

}
