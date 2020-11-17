using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace PSmash.Resources
{
    public class CoveredArea : MonoBehaviour
    {
        [SerializeField] float fadeOutSpeed = 1;
        [SerializeField] Tilemap tileMap = null;
        [SerializeField] AudioSource audioSource = null;

        private void Awake()
        {
            transform.GetChild(0).gameObject.SetActive(true);
        }
        public void ShowHiddenArea()
        {
            StartCoroutine(ThisTileMapFadeOut());
        }

        public void ShowSecretArea()
        {
            audioSource.Play();
            StartCoroutine(ThisTileMapFadeOut());
        }

        public IEnumerator ThisTileMapFadeOut()
        {
            float alpha = tileMap.color.a;
            tileMap.GetComponent<TilemapCollider2D>().enabled = false;
            while (alpha > 0)
            {
                alpha -= Time.deltaTime * fadeOutSpeed;
                if (alpha < 0) alpha = 0;
                tileMap.color = new Color(tileMap.color.r, tileMap.color.g, tileMap.color.b, alpha);
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }
    }
}



