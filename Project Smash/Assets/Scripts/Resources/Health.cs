using UnityEngine;

namespace PSmash.Resources
{
    public class Health : MonoBehaviour, IDamagable,IKillable
    {
        [Header("General")]
        [SerializeField] protected int health = 100;
        [SerializeField] protected AudioSource audioSource;
        protected bool isDead = false;
        protected int initialHealth;

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

