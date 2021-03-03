using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Combat.Weapons;

namespace PSmash.Attributes
{
    public interface IDamagable
    {
        void TakeDamage(Transform attacker, Weapon weapon, float damage);
    }
}


