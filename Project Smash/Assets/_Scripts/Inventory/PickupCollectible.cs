using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Inventories
{
    public class PickupCollectible : MonoBehaviour
    {
        [SerializeField] Collider2D collectingCollider = null;
        [SerializeField] Transform body;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collectingCollider.enabled = false;
                StartCoroutine(CollectCollectible());
            }
        }

        IEnumerator CollectCollectible()
        {
            body.gameObject.SetActive(false);
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.Play();
            while (audioSource.isPlaying)
            {
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}

