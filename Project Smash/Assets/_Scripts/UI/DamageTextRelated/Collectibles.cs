using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PSmash.Items;

namespace PSmash.UI
{
    public class Collectibles : MonoBehaviour
    {
        [SerializeField] int timeToFadeOut = 2;
        [SerializeField] Text maxStarsText= null;
        [SerializeField] Text currentStarsText = null;

        int currentStars=0;
        // Start is called before the first frame update
        CanvasGroup canvasGroup;
        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        void Start()
        {
            canvasGroup.alpha = 0;
            currentStarsText.text = currentStars.ToString();
            maxStarsText.text = Star.activeStarsOnSceneQuantity.ToString();
        }

        private void OnEnable()
        {
            Star.OnStarCollected += StarObtained;
        }

        private void OnDisable()
        {
            Star.OnStarCollected -= StarObtained;
        }

        void StarObtained()
        {
            canvasGroup.alpha = 1;
            currentStars++;
            currentStarsText.text = currentStars.ToString();
            StartCoroutine(FadeOut());
        }

        IEnumerator FadeOut()
        {
            float alpha = canvasGroup.alpha;
            yield return new WaitForSeconds(2);
            while(alpha > 0)
            {
                alpha -= Time.deltaTime / timeToFadeOut;
                if (alpha < 0) alpha = 0;
                canvasGroup.alpha = alpha;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}


