using PSmash.Combat;
using PSmash.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] Collider2D playerSpotterTrigger = null;
    EnemyMovement movement;
    Transform playerTransform;
    EnemyAttack attack;
    EnemyHealth health;

    bool isPlayerSpotted = false;
    bool isPlayerReachable = true;
    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<EnemyMovement>();
        playerTransform = PlayerController.playerTransform;
        if (playerTransform == null) Debug.LogWarning("Player Not Found");
        attack = GetComponent<EnemyAttack>();
        health = GetComponent<EnemyHealth>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (health.IsDead()) return;
        if (health.IsBeingInterruptedByDamage()) 
        { 
            //Debug.Log("Not Moving"); 
            return; 
        }
        if (!isPlayerSpotted /*&& movement.IsPlayerReachable()*/) return;
        AttackBehavior();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerSpotted = true;
            playerSpotterTrigger.enabled = false;
            //Debug.Log("PlayerSpotted");
        }
    }

    void AttackBehavior()
    {
        attack.SetTarget(playerTransform);
        //Debug.Log(playerTransform);
    }

    bool InAttackRange()
    {
        bool inAttackRange = Mathf.Abs(playerTransform.position.x - transform.position.x) < attackRange;
        return inAttackRange;
    }
}
