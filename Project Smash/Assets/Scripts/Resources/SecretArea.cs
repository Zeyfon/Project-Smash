using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PSmash.Resources
{
    public class SecretArea : MonoBehaviour
    {
        [SerializeField] float fadeOutSpeed = 1;
        [SerializeField] Tilemap tileMap = null;
        [SerializeField] AudioSource audioSource = null;
        // Start is called before the first frame update

        public IEnumerator SecretAreaDiscoveredMoment()
        {
            print("Started this");
            audioSource.Play();
            yield return ThisTileMapFadeOut();
        }

        public void ShowHiddenArea()
        {
            StartCoroutine(ThisTileMapFadeOut());
        }

        public IEnumerator ThisTileMapFadeOut()
        {
            float alpha = tileMap.color.a;
            while (alpha > 0)
            {
                alpha -= Time.deltaTime * fadeOutSpeed;
                if (alpha < 0) alpha = 0;
                tileMap.color = new Color(tileMap.color.r, tileMap.color.g, tileMap.color.b, alpha);
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }

        public void Secretmoment()
        {
            StartCoroutine(SecretAreaDiscoveredMoment());
        }
    }
}



