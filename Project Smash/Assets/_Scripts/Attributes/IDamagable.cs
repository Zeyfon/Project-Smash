using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Combat.Weapons;
using PSmash.Combat;

namespace PSmash.Attributes
{
    public interface IDamagable
    {
        void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage, float attackForce);
    }
}


