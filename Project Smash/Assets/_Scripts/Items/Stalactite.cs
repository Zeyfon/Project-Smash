using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;
using PSmash.Combat.Weapons;

namespace PSmash.Items
{
    public class Stalactite : MonoBehaviour, IDamagable
    {
        [Header("CONFIG")]
        [SerializeField] bool isDestroyedByMace = false;

        [Header("Material")]
        [SerializeField] Weapon weaknessWeapon = null;
        [SerializeField] Sprite destroyedSprite = null;
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] AudioClip destroyedClip = null;
        [SerializeField] AudioClip damagedClip = null;


        public void TakeDamage(Transform attacker, Weapon weapon, float damage)
        {
            audioSource.PlayOneShot(damagedClip);
            if (weapon == this.weaknessWeapon)
            {
                if (isDestroyedByMace)
                {
                    Break();
                }
            }
        }

        void Break()
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponentInChildren<SpriteRenderer>().sprite = destroyedSprite;
            audioSource.clip = destroyedClip;
            audioSource.Play();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!isDestroyedByMace && collision.collider.GetComponent<Boulder>())
            {
                Break();
            }
        }
    }

}
