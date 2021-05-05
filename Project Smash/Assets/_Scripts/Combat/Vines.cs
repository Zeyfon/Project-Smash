using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;
using PSmash.Combat.Weapons;
using PSmash.Combat;

namespace PSmash.Tools
{
    public class Vines : MonoBehaviour, IDamagable
    {
        [SerializeField] Weapon weaknessWeapon = null;
        [SerializeField] AudioSource noDamageAudio = null;
        [SerializeField] AudioSource damageAudio = null;
        public void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage, float attackForce)
        {
            print("Attacker  " + attacker + "  weapon   " + weapon + "  damage  " + damage);
            if(weapon == weaknessWeapon)
            {
                DoDamage();

            }
            else
            {
                DoNoDamage();
            }
        }

        void DoNoDamage()
        {
            noDamageAudio.Play();
        }

        void DoDamage()
        {
            damageAudio.Play();
        }
    }
}

