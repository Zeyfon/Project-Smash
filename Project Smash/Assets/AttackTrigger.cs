using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;

public class AttackTrigger : MonoBehaviour
{
    [SerializeField] Collider2D collider = null;
    int damage = 0;

    public void Attack(int damage)
    {
        this.damage = damage;
        collider.enabled = true;
    }

    public void DisableAttackTrigger()
    {
        collider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            print("Attack Trigger worked");
            collision.GetComponent<IDamagable>().TakeDamage(transform.parent, damage);
            collider.enabled = false;
        }
    }
}
