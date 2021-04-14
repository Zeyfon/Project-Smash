using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{
    public class ArmoredEnemy : MonoBehaviour
    {
        [SerializeField] float unarmoredSpeedFactor;
        [SerializeField] float unarmoredAttackSpeedFactor;

        public float GetSpeedFactorModifier()
        {
            return unarmoredSpeedFactor;
        }

        public float GetAttackSpeedFactor()
        {
            return unarmoredAttackSpeedFactor;
        }
    }

}
