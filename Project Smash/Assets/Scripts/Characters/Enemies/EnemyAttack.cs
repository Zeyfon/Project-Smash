using PSmash.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] float attackRange=2;
    [SerializeField] Transform attackTransform=null;
    [SerializeField] float attackRadius = 1;
    [SerializeField] int damage = 1;
    [SerializeField] PhysicsMaterial2D fullFriction = null;
    [SerializeField] PhysicsMaterial2D lowFriction = null;
    [SerializeField] float getParriedDistance = 2;
    [SerializeField] LayerMask whatIsPlayerGuard;
    [SerializeField] LayerMask whatIsPlayer;
    [SerializeField] AudioClip parriedSound;
    Animator animator;
    EnemyMovement movement;
    PlayerHealth target = null;
    EnemyHealth health;
    Rigidbody2D rb;
    Coroutine coroutine;
    AudioSource audioSource;
    bool isAttacking = false;
    bool canAttack = true;
    bool canBeParried = false;
    bool isParried = false;
    
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<EnemyMovement>();
        health = GetComponent<EnemyHealth>();
        rb = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (health.IsDead()) return;
        if (health.IsInterrupted()) return;
        if (target == null) return;
        //Debug.Log(target);
        if (!canAttack) return;

        if (!IsTargetInRange())
        {
            if (PlayerIsAbove()) 
            {
                MoveMeAwayFromPlayer();
                return;
            }
            GetComponent<EnemyMovement>().MoveTo(target.transform.position, 1f);
            //Debug.Log("Player Outside attack range");
        }
        else
        {
            canAttack = false;
            AttackBehaviour();
            GetComponent<EnemyMovement>().Cancel();
        }
    }

    bool PlayerIsAbove() 
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 2.3f,whatIsPlayer);
        if (hit)
        {
            Debug.Log(hit.transform.gameObject);
            return true;
        }
        else return false;
    }
   
    void MoveMeAwayFromPlayer()
    {
        float playerRelativePosition = target.transform.position.x - transform.position.x;
        if(playerRelativePosition > 0)
        {
            rb.MovePosition(new Vector2(0.1f, 0) + (Vector2)transform.position);
        }
        else rb.MovePosition(new Vector2(-0.1f, 0) + (Vector2)transform.position);

    }

    void AttackBehaviour()
    {
        if (!isAttacking) StartCoroutine(Attack());
        isAttacking = true;    
    }
    bool IsTargetInRange()
    {
        bool isInRange = Mathf.Abs(target.transform.position.x- transform.position.x) < attackRange;
        //bool isInRange = Mathf.Abs(Vector2.Distance(target.transform.position, transform.position)) < attackRange;
        //Debug.Log(isInRange);
        return isInRange;
    }
    public void AttackTarget()
    {
        movement.Cancel();
        StartCoroutine(Attack());
    }
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform.GetComponent<PlayerHealth>();
        //Debug.Log(target);
    }
    IEnumerator Attack()
    {
        rb.sharedMaterial = fullFriction;
        isParried = false;
        movement.CheckFlip();
        animator.SetInteger("Attack", 1);
        while(animator.GetInteger("Attack")!= 100)
        {
            yield return new WaitForEndOfFrame();
        }
        animator.SetInteger("Attack", 0);
        yield return new WaitForSeconds(1);
        rb.sharedMaterial = lowFriction;
        canAttack = true;
        isAttacking = false;
        //print("CanDo Attack Behaviour again");
    }

    //Animation Event

    void LookingForPlayerGuard()
    {
        canBeParried = true;
        coroutine = StartCoroutine(LookForPlayerGuard());
    }

    IEnumerator LookForPlayerGuard()
    {
        while (canBeParried)
        {            
            RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + new Vector2(0,0.5f), transform.right, getParriedDistance, whatIsPlayerGuard);
            Debug.DrawRay(transform.position + new Vector3(0, 0.5f, 0), transform.right);
            if (!hit)
            {
                Debug.DrawRay(transform.position + new Vector3(0, -0.5f, 0), transform.right);
                hit = Physics2D.Raycast((Vector2)transform.position + new Vector2(0, -0.5f), transform.right, getParriedDistance, whatIsPlayerGuard);
            }
            if (hit)
            {
                //Debug.Break();
                Debug.Log(gameObject + "  Got Parried");
                audioSource.PlayOneShot(parriedSound);
                canBeParried = false;
                isParried = true;
                GetComponent<EnemyHealth>().DamageTaken(hit.collider.transform.parent.transform, hit.collider.transform.GetComponent<PlayerGuard>().GetParryDamage());
                yield break;
            }
            yield return new WaitForSeconds(0.03f);
        }
    }
    void AttackEvent()
    {
        //print("Check if Parried");
        if (isParried) return;
        if(coroutine != null) canBeParried = false;
        PlayerHealth targetHealth;
        Collider2D[] colls = Physics2D.OverlapCircleAll(attackTransform.position, attackRadius);
        //Debug.Log(colls);
        foreach(Collider2D coll in colls)
        {
            targetHealth = coll.GetComponent<PlayerHealth>();
            if (targetHealth == null) continue;          
            if (targetHealth.transform == transform) return;
            targetHealth.SendDamage(transform.position, damage); 
        }
    }
}
