using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{
    public class AttackTrigger : MonoBehaviour
    {
        [SerializeField] Collider2D myAttackTrigger = null;
        int damage = 0;

        public void Attack(int damage)
        {
            this.damage = damage;
            myAttackTrigger.enabled = true;
        }

        public void DisableAttackTrigger()
        {
            myAttackTrigger.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                print("Attack Trigger worked");
                collision.GetComponent<IDamagable>().TakeDamage(transform.parent, damage);
                myAttackTrigger.enabled = false;
            }
        }
    }
}
