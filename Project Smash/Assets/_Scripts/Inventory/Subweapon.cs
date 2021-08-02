using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Inventories
{
    [CreateAssetMenu(menuName = "Items/SubWeapon")]
    public class Subweapon : Weapon
    {
        [SerializeField] int myLevel;
        public int GetMyLevel()
        {
            return myLevel;
        }
    }
}
