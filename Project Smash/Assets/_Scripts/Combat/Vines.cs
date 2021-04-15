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
        [SerializeField] AudioSource noDamageAudio = null;
        [SerializeField] AudioSource damageAudio = null;
        public void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage)
        {
            print("Attacker  " + attacker + "  weapon   " + weapon + "  damage  " + damage);
            switch (weapon.weapon)
            {
                case WeaponList.Saber:
                    DoDamage();
                    break;
                default:
                    NoDamage();
                    break;
            }
        }

        void NoDamage()
        {
            noDamageAudio.Play();
        }

        void DoDamage()
        {
            damageAudio.Play();
        }
    }
}

