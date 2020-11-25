using PSmash.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;

namespace PSmash.Combat.Weapons
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] GameObject enemyHitEffect = null;
        [SerializeField] GameObject wallHitEWffect = null;
        [SerializeField] float speed = 1;
        [SerializeField] int damage = 10;
        [SerializeField] AudioClip wallHitSound = null;
        [SerializeField] AudioClip enemyHitSound = null;

        AudioSource audioSource;
        Rigidbody2D rb;
        bool hasHit = false;
        // Start is called before the first frame update
        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            rb = GetComponent<Rigidbody2D>();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (hasHit) return;
            float deltaPosX = transform.right.x *speed * Time.fixedDeltaTime;
            Vector2 newPosition = transform.position + new Vector3(deltaPosX, 0);
            rb.MovePosition(newPosition);
        }

        public void SetData(bool isLookingRight)
        {
            if (isLookingRight) return;
            else transform.eulerAngles = new Vector3(0, 180, 0);
        }

        //Anim Event
        void WallHitSound()
        {
            print(gameObject.name + "  Wall Hit Sound");
            audioSource.clip = wallHitSound;
            audioSource.pitch = Random.Range(0.8f, 1f);
            audioSource.Play();
        }

        //Anim Event
        void EnemyHitSound()
        {
            print(gameObject.name + "  Enemy Hit Sound");
            audioSource.clip = enemyHitSound;
            audioSource.pitch = Random.Range(0.8f, 1f);
            audioSource.Play();
        }

        void DestroyObject()
        {
            Destroy(gameObject);
        }

        //public void IslookingRight(Vector3 isRight)
        //{
        //    if (isRight == transform.right)
        //    {
        //        lookingRight = true;
        //    }
        //    else lookingRight = false;
        //}
        //private float GetCosArgument()
        //{
        //    float x;
        //    timer += Time.deltaTime;
        //    x = (timer * speed) + Mathf.PI / 2;
        //    if (x >= (3f / 2f) * Mathf.PI)
        //    {
        //        Destroy(gameObject);
        //        return x = (3f / 2f) * Mathf.PI;
        //    }
        //    else
        //    {
        //        return x;
        //    }
        //}

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player")) return;
            GetComponent<Collider2D>().enabled = false;
            //print("Collider Disabled");


            if (collision.CompareTag("Enemy"))
            {
                hasHit = true;
                Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
                GetComponent<Animator>().SetTrigger("EnemyHit");
                collision.GetComponent<IDamagable>().TakeDamage(transform, damage);
                //StartCoroutine(EnemyHit(collision.transform));
                //print(gameObject.name + "  Hit Enemy");
            }
            else
            {
                //print(collision.name);
                //print(gameObject.name + "  Hit Wall");
                hasHit = true;
                Instantiate(wallHitEWffect, transform.position + new Vector3(GetComponent<BoxCollider2D>().size.x/2*transform.right.x,0), Quaternion.identity);
                GetComponent<Animator>().SetTrigger("WallHit");
                //collision.GetComponent<IDamagable>().TakeDamage(transform, damage);
            }
        }

        IEnumerator EnemyHit(Transform targetTransform)
        {

            while (Mathf.Abs(Vector2.Distance(targetTransform.position, transform.position)) > 0.5f)
            {
                print(Vector2.Distance(targetTransform.position, transform.position));
                yield return null;
            }
            hasHit = true;
            Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
            GetComponent<Animator>().SetTrigger("EnemyHit");
            targetTransform.GetComponent<IDamagable>().TakeDamage(transform, damage);
        }
    }
}


