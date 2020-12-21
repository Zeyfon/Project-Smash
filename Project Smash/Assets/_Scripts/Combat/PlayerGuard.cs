using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;

namespace PSmash.Combat
{
    public class PlayerGuard : MonoBehaviour, IDamagable
    {
        [SerializeField] int damage = 30;
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] AudioClip parrySound = null;
        [SerializeField] AudioClip guardSound = null;

        bool canParry=false;

        public void TakeDamage(Transform attacker, int damage)
        {
            if (canParry)
            {
                //attacker.GetComponent<EnemyHealth>().SetIsParried(true);
                attacker.GetComponent<IDamagable>().TakeDamage(transform.parent, damage);
                transform.parent.GetComponent<PlayerFighter>().StartParry();
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
            if (this.canParry)
            { 
                //print("Started CanParry");
            }
            if (!this.canParry) 
            { 
                //print("Finished CanParry");
            }
        }
    }
}

