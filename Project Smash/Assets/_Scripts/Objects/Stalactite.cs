using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;
using PSmash.Inventories;
using PSmash.Combat;
using GameDevTV.Saving;

namespace PSmash.Items
{
    public class Stalactite : MonoBehaviour, IDamagable, ISaveable
    {

        //CONFIG
        [Header("CONFIG")]
        [SerializeField] bool isDestroyedByMace = false;

        [Header("Material")]
        [SerializeField] Weapon weaknessWeapon = null;
        [SerializeField] Sprite destroyedSprite = null;
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] AudioClip destroyedClip = null;
        [SerializeField] AudioClip damagedClip = null;


        //////////////////////////////////////////////PUBLIC////////////////////////////////////////
        public void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage, float attackForce)
        {
            audioSource.PlayOneShot(damagedClip);
            if (weapon == this.weaknessWeapon)
            {
                if (isDestroyedByMace)
                {
                    Break();
                    BreakSound();
                }
            }
        }

        /////////////////////////////////////////////PRIVATE/////////////////////////////////////////

        void Break()
        {
            GetComponent<Collider2D>().enabled = false;
            GetComponentInChildren<SpriteRenderer>().sprite = destroyedSprite;
        }

        void BreakSound()
        {
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

        //////////////////////////SAVE SYSTEM/////////////////

        public object CaptureState()
        {
            bool isDestroyed;
            if (!GetComponent<Collider2D>().enabled)
                isDestroyed = true;
            else
                isDestroyed = false;
            return isDestroyed;
        }

        public void RestoreState(object state, bool isLoadLastSavedScene)
        {
            bool isDestroyed = (bool)state;
            if (isDestroyed)
                Break();
        }
    }

}
