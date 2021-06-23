using PSmash.Attributes;
using PSmash.Inventories;
using PSmash.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Items
{
    public class OreCluster : MonoBehaviour, IDamagable
    {
        [SerializeField] float health = 30;
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] AudioClip damageSound = null;
        [SerializeField] GameObject drop = null;
        public void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage, float attackForce)
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

