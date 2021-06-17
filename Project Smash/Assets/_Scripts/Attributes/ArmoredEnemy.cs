using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{
    public class ArmoredEnemy : MonoBehaviour
    {
        [SerializeField] float unarmoredMovementSpeedModifier;
        [SerializeField] float unarmoredAttackSpeedModifier;

        public float GetSpeedFactorModifier()
        {
            return unarmoredMovementSpeedModifier;
        }

        public float GetAttackSpeedModifier()
        {
            return unarmoredAttackSpeedModifier;
        }
    }

}
