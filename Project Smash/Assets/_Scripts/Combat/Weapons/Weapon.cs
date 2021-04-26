using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Combat;

namespace PSmash.Combat.Weapons
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Weapons")]
    public class Weapon : ScriptableObject
    {
        public float damage;
        public float damagePenetrationValue;
        //public WeaponList weapon;
    }
}

