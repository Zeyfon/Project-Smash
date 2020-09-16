using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Resources
{
    public interface IDamagable
    {
        void TakeDamage(Transform attacker, int damage);
    }
}


