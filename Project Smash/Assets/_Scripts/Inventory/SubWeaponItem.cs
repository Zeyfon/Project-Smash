using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Inventories
{
    [CreateAssetMenu(menuName = "Items/SubWeapon Item")]
    public class SubWeaponItem : Item
    {
        [SerializeField] int animatorIntValue = 0;
        [SerializeField] AudioClip useItemClip = null;


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
