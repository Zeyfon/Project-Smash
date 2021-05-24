using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Core;
namespace PSmash.SceneManagement
{
    public class UIFader : MonoBehaviour
    {
        [SerializeField] CanvasGroup canvasGroup = null;

        /// <summary>
        /// Turns the screen from black to colored in the given time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public IEnumerator FadeIn(float time)
        {
            //print("Fading in");
            Fader fader = new Fader();
            yield return fader.FadeIn(canvasGroup, time);
        }

        /// <summary>
        /// Turns the screen from colored to black in the given time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public IEnumerator FadeOut(float time)
        {
            //print("Fading Out");
            Fader fader = new Fader();
            yield return fader.FadeOut(canvasGroup, time);
        }
        
        /// <summary>
        /// Turns the screen to black inmediately
        /// </summary>
        public void FadeOutInmediate()
        {
            Fader fader = new Fader();
            fader.InstantFadeOut(canvasGroup);
        }
    }

}
