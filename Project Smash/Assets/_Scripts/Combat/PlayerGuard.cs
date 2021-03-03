using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;
using PSmash.Combat.Weapons;

namespace PSmash.Combat
{
    public class PlayerGuard : MonoBehaviour, IDamagable
    {
        [SerializeField] int damage = 30;
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] AudioClip parrySound = null;
        [SerializeField] AudioClip guardSound = null;
        [SerializeField] float parryTime = 1;

        bool canParry=false;
        float parryTimer = 0;
        PlayMakerFSM pm;

        void Update()
        {
            if (parryTimer < parryTime)
                canParry = true;
            else
                canParry = false;
            parryTimer += Time.deltaTime;
        }

        public void TakeDamage(Transform attacker, Weapon weapon, float damage)
        {
            if (canParry)
            {
                attacker.GetComponent<IDamagable>().TakeDamage(transform.parent, weapon, damage);
                pm.SendEvent("PARRY");
                PlaySound(parrySound);
            }
            else
            {
                PlaySound(guardSound);
            }
        }

        private void PlaySound(AudioClip sound)
        {
            audioSource.clip = sound;
            audioSource.pitch = Random.Range(0.7f, 1);
            audioSource.Play();
        }

        public void SetCanParry(bool canParry)
        {
            this.canParry = canParry;
        }

        public void EnableParryWindow(PlayMakerFSM pm)
        {
            this.pm = pm;
            parryTimer = 0;
        }
    }
}

