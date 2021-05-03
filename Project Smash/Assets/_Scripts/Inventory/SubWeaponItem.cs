using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Inventories
{
    [CreateAssetMenu(menuName = "Items/SubWeapon")]
    public class SubWeaponItem : Item
    {
        [Header("Animation Info")]

        [SerializeField] int animatorIntValue = 0;
        [SerializeField] AudioClip useItemClip = null;

        [Header("DamageValues")]
        [SerializeField] float damage;
        [SerializeField] float damagaPenetrationValue;


        public float GetDamage()
        {
            return damage;
        }

        public float GetDamagePenetrationValue()
        {
            return damagaPenetrationValue;
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
