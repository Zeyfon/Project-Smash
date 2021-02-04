using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;

namespace PSmash.Tools
{
    public class Vines : MonoBehaviour, IDamagable
    {
        [SerializeField] AudioSource noDamageAudio = null;
        [SerializeField] AudioSource damageAudio = null;
        public void TakeDamage(Transform attacker, WeaponList weapon, float damage)
        {
            print("Attacker  " + attacker + "  weapon   " + weapon + "  damage  " + damage);
            switch (weapon)
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

