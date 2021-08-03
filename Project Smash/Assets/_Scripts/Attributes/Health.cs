using PSmash.Inventories;
using PSmash.Combat;
using UnityEngine;
using UnityEngine.Events;

namespace PSmash.Attributes
{
    public class Health : MonoBehaviour, IDamagable,IKillable
    {
        [Header("General")]
        [SerializeField] protected TakeDamageEvent onTakeDamage;
        [SerializeField] protected UnityEvent onDead;

        protected float health = 0;
        protected AudioSource audioSource;
        protected bool isDead = false;
        //protected int initialHealth;

        public enum CriticalType
        {
            Critical, NoCritical
        }

        public enum DamageType
        {
            Posture,Health
        }

        public class DamageSlot
        {
            public float damage;
            public DamageType damageType;
            public CriticalType criticalType;
        }

        [System.Serializable]
        public class TakeDamageEvent : UnityEvent<DamageSlot>
        {

        }
        public virtual void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage, float attackForce)
        {
            //Used by child class
        }

        public virtual void Kill(Transform attacker)
        {
            //Used by child class
        }

        public virtual bool IsDead()
        {
            return isDead;
        }
    }
}

