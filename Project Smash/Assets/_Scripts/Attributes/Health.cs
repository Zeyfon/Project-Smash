﻿using UnityEngine;
using UnityEngine.Events;

namespace PSmash.Attributes
{
    public class Health : MonoBehaviour, IDamagable,IKillable
    {
        [Header("General")]
        [SerializeField] protected TakeDamageEvent onTakeDamage;

        protected int health;
        protected AudioSource audioSource;
        protected bool isDead = false;
        //protected int initialHealth;

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<float>
        {

        }
        public virtual void TakeDamage(Transform attacker, int damage)
        {
            //Used by child class
        }

        public virtual void Kill(Transform attacker)
        {
            //Used by child class
        }
    }
}

