using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Core;

namespace PSmash.Temporal
{
    public class EndTest : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                StartCoroutine(EndGame());
            }
        }

        IEnumerator EndGame()
        {
            //Fader fader = FindObjectOfType<Fader>();
            Fader fader = new Fader();
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            yield return fader.FadeIn(canvasGroup, 1);
            Application.Quit();
        }
    }
}

