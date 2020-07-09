using PSmash.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 1;
    [SerializeField] float maxDistance = 1;
    [SerializeField] bool lookingRight = true;
    [SerializeField] int damage = 10;
    Rigidbody2D rb;
    float timer = 0;
    float initialXPosition;
    bool hasPassed = false;
    // Start is called before the first frame update
    void Start()
    {
        initialXPosition = transform.position.x;
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        float timer = 0;
        while (!hasPassed)
        {
            timer += Time.deltaTime;
            if (timer > 0.3f) hasPassed = true;
            yield return new WaitForEndOfFrame();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float x = Mathf.Cos(GetCosArgument());
        if (lookingRight) x = -x;
        rb.MovePosition(new Vector2(initialXPosition + x  * maxDistance, transform.position.y)) ;
    }

    public void IslookingRight(Vector3 isRight)
    {
        if (isRight == transform.right)
        {
            lookingRight = true;
        }
        else lookingRight = false;
    }
    private float GetCosArgument()
    {
        float x;
        timer += Time.deltaTime;
        x = (timer* speed)+Mathf.PI/2;
        if (x >= (3f / 2f) * Mathf.PI)
        {
            Destroy(gameObject);
            return x = (3f / 2f) * Mathf.PI;
        }
        else
        {
            return x;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            //Debug.Log("Sending Damage to Enemy");
            collision.GetComponent<EnemyHealth>().DamageTaken(transform, damage);
        }
        if (collision.CompareTag("Player"))
        {
            if (hasPassed)
            {
                //Debug.Log("Being recovered by player");
                Destroy(gameObject);
            }
        }
    }
}
