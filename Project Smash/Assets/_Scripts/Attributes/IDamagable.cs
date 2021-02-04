﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{
    public interface IDamagable
    {
        void TakeDamage(Transform attacker, WeaponList weapon, float damage);
    }
}


