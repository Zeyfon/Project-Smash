using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Inventories
{
    [CreateAssetMenu(menuName = "Items/SubWeapon")]
    public class Weapon : Item
    {
        [Header("Animation Info")]

        [SerializeField] int animatorIntValue = 0;
        [SerializeField] AudioClip useItemClip = null;

        [Header("DamageValues")]
        [SerializeField] float damage;
        [Range(0,2)]
        [SerializeField] float attackForce;
        [Range(0,1f)]
        [SerializeField] float attackForceTime;

        public float GetDamage()
        {
            return damage;
        }

        public float GetAttackForce()
        {
            return attackForce;
        }

        public float GetAttackForceTime()
        {
            return attackForceTime;
        }

        public int GetAnimatorInt()
        {
            return animatorIntValue;
        }
        public AudioClip GetAudioClip()
        {
            return useItemClip;
        }

    }
}
