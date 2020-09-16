using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Resources
{
    public interface IKillable
    {
        void Kill(Transform attacker);
    }
}

