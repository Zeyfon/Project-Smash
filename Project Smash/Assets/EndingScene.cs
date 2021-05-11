using PSmash.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingScene : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup = null;
    [SerializeField] float fadeIntTime = 3;
    [SerializeField] float waitingTime = 8;
    [SerializeField] float fadeOutTime = 3;
    // Start is called before the first frame update
    void Awake()
    {
        StartCoroutine(Ending());
    }

    IEnumerator Ending()
    {
        Fader fader = new Fader();
        yield return fader.InstantFadeOut(canvasGroup);
        yield return new WaitForSeconds(2);
        yield return fader.FadeOut(canvasGroup, fadeOutTime);
        yield return new WaitForSeconds(waitingTime);
        yield return fader.FadeIn(canvasGroup, fadeIntTime);
        QuitGame();
    }

    void QuitGame()
    {
        print("Application Quit");
        Application.Quit();
    }

}
