using PSmash.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Items
{
    public class OreCluster : MonoBehaviour, IDamagable
    {
        [SerializeField] int health = 30;
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] AudioClip damageSound = null;
        [SerializeField] GameObject drop = null;
        public void TakeDamage(Transform attacker, int damage)
        {
            print("Ore Damaged");
            health -= damage;
            audioSource.PlayOneShot(damageSound);
            if (health <= 0)
            {
                GetComponent<Collider2D>().enabled = false;
                StartCoroutine(DestroyObject());
            }
        }

        IEnumerator DestroyObject()
        {
            audioSource.Play();
            for (int i = 0; i < 2; i++)
            {
                Instantiate(drop, transform.position, Quaternion.identity);
            }
            while (audioSource.isPlaying)
            {
                yield return null;
            }
            Destroy(gameObject);
        }
    }
}

