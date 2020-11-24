using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{
    public interface IKillable
    {
        void Kill(Transform attacker);
    }
}

