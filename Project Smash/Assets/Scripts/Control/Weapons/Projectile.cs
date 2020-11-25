using PSmash.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;

namespace PSmash.Combat.Weapons
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] float speed = 1;
        [SerializeField] int damage = 10;
        [SerializeField] AudioClip wallHitSound = null;
        [SerializeField] AudioClip enemyHiySound = null;

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
            audioSource.clip = wallHitSound;
            audioSource.pitch = Random.Range(0.8f, 1f);
            audioSource.Play();
        }

        //Anim Event
        void EnemyHitSound()
        {
            audioSource.clip = enemyHiySound;
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
            hasHit = true;

            if (collision.CompareTag("Enemy"))
            {
                print(gameObject.name + "  Hit Enemy");
                GetComponent<Animator>().SetTrigger("EnemyHit");
                collision.GetComponent<IDamagable>().TakeDamage(transform, damage);
            }
            else
            {
                print(collision.name);
                print(gameObject.name + "  Hit Wall");
                GetComponent<Animator>().SetTrigger("WallHit");
                //collision.GetComponent<IDamagable>().TakeDamage(transform, damage);
            }
        }
    }
}


