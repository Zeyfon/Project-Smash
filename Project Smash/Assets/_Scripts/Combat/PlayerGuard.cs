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
        
        PlayMakerFSM guardFSM = null;
        bool canParry=false;
        float parryTimer = 0;

        void Awake()
        {
            foreach(PlayMakerFSM pm in GetComponentsInParent<PlayMakerFSM>())
            {
                if(pm.FsmName == "GuardParryState")
                {
                    print("Found guard fsm");
                    guardFSM = pm;
                }
            }
        }

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
                guardFSM.SendEvent("PARRY");
                PlaySound(parrySound);
            }
            else
            {
                PlaySound(guardSound);
            }
        }

        public void EnableParryWindow()
        {
            parryTimer = 0;
        }

        void PlaySound(AudioClip sound)
        {
            audioSource.clip = sound;
            audioSource.pitch = Random.Range(0.7f, 1);
            audioSource.Play();
        }
    }
}

