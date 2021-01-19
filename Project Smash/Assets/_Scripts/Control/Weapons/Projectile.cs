using PSmash.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;

namespace PSmash.Combat.Weapons
{
    public class Projectile : MonoBehaviour, IDamagable
    {
        [SerializeField] GameObject enemyHitEffect = null;
        [SerializeField] GameObject wallHitEWffect = null;
        [SerializeField] float speed = 1;
        [SerializeField] int damage = 10;
        [SerializeField] AudioClip wallHitSound = null;
        [SerializeField] AudioClip enemyHitSound = null;
        [SerializeField] bool isHomingProyectile = false;
        [SerializeField] float rotateSpeed = 5;
        [SerializeField] float parriedSpeed = 15;

        float currentSpeed;
        AudioSource audioSource;
        Rigidbody2D rb;
        Health owner;
        bool hasHit = false;
        Transform target;
        // Start is called before the first frame update
        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            rb = GetComponent<Rigidbody2D>();
        }
        void Start()
        {
            currentSpeed = speed;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (hasHit) 
            {
                return;
            }
            //print("Moving");
            if (target != null)
            {
                //print("Rotating towards " + target.gameObject.name);
                Vector2 direction = (Vector2)target.position - rb.position;
                direction.Normalize();
                float rotateAmount = Vector3.Cross(direction, transform.right).z;
                rb.angularVelocity = -rotateAmount * rotateSpeed;
            }
                rb.velocity = transform.right * currentSpeed;
        }

        public void SetData(bool isLookingRight, Health owner)
        {
            this.owner = owner;
            if (isLookingRight) return;
            else transform.eulerAngles = new Vector3(0, 180, 0);
        }

        //Used by the Attack FSM of the Ranger01
        public void SetTarget(GameObject targetGameObject, Health owner)
        {
            this.owner = owner;
            Vector2 direction = ((targetGameObject.transform.position + new Vector3(0,1,0)) - transform.position).normalized;
            print(direction);
            float angle = Mathf.Atan(direction.y / direction.x)*Mathf.Rad2Deg;
            print(angle);
            if(direction.y>0 && direction.x > 0)
            {
                print("Is at first cuadrant");
               //Do nothing
            }
            else if(direction.y>0 && direction.x < 0)
            {
                print("Is at second cuadrant");
                angle = 180 + angle;
            }
            else if(direction.y<0 && direction.x <0)
            {
                print("Is at third cuadrant");
                angle = 180 + angle;
            }
            else
            {
                print("Is at fourth cuadrant");
                //angle *= -1;
            }
            transform.rotation = Quaternion.Euler(0, 0, angle);
            print(angle);
        }

        //Anim Event
        void WallHitSound()
        {
            //print(gameObject.name + "  Wall Hit Sound");
            audioSource.clip = wallHitSound;
            audioSource.pitch = Random.Range(0.8f, 1f);
            audioSource.Play();
        }

        //Anim Event
        void NPCHitSound()
        {
            //print(gameObject.name + "  Enemy Hit Sound");
            audioSource.clip = enemyHitSound;
            audioSource.pitch = Random.Range(0.8f, 1f);
            audioSource.Play();
        }

        void DestroyObject()
        {
            Destroy(gameObject);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (hasHit)
                return;
            string tag = owner.gameObject.tag;
            if(tag == "Player")
            {
                if (collision.CompareTag("Enemy"))
                {
                    print("Attack from Player colliding with enemy");
                    hasHit = true;
                    Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
                    collision.GetComponent<IDamagable>().TakeDamage(transform, damage);
                    if (GetComponent<Animator>() != null)
                        GetComponent<Animator>().SetTrigger("TargetHit");
                    NPCHitSound();
                    StartCoroutine(DestroyGameObject());
                    rb.velocity = new Vector2(0, 0);
                    rb.angularVelocity = 0;
                }
                else if (collision.CompareTag("Ground"))
                {
                    print("Attack from player colliding with ground");
                    hasHit = true;
                    Instantiate(wallHitEWffect, transform.position + new Vector3(GetComponent<BoxCollider2D>().size.x / 2 * transform.right.x, 0), Quaternion.identity);
                    if (GetComponent<Animator>() != null)
                        GetComponent<Animator>().SetTrigger("WallHit");
                    WallHitSound();
                    StartCoroutine(DestroyGameObject());
                    rb.velocity = new Vector2(0, 0);
                    rb.angularVelocity = 0;
                }
                else
                {
                    Debug.LogWarning(gameObject.name + " is from player and is not supposed to collide with  " + collision.gameObject.name);
                }
            }
            else
            {
                if (collision.CompareTag("Player"))
                {
                    print("Attack from enemy colliding with player");
                    hasHit = true;
                    Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
                    collision.GetComponent<IDamagable>().TakeDamage(transform, damage);
                    if (GetComponent<Animator>() != null)
                        GetComponent<Animator>().SetTrigger("TargetHit");
                    NPCHitSound();
                    StartCoroutine(DestroyGameObject());
                    rb.velocity = new Vector2(0, 0);

                }
                else if (collision.GetComponent<PlayerGuard>())
                {
                    print("Attack from enemy colliding with guard");
                    hasHit = true;
                    Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
                    collision.GetComponent<IDamagable>().TakeDamage(transform, damage);
                    if (GetComponent<Animator>() != null)
                        GetComponent<Animator>().SetTrigger("GuardHit");
                    rb.velocity = new Vector2(0, 0);
                    StartCoroutine(DestroyGameObject());
                }
                else if (collision.CompareTag("Ground"))
                {
                    print("Attack from enemy colliding with ground");
                    hasHit = true;
                    Instantiate(wallHitEWffect, transform.position + new Vector3(GetComponent<BoxCollider2D>().size.x / 2 * transform.right.x, 0), Quaternion.identity);
                    if (GetComponent<Animator>() != null)
                        GetComponent<Animator>().SetTrigger("WallHit");
                    WallHitSound();
                    StartCoroutine(DestroyGameObject());
                    rb.velocity = new Vector2(0, 0);
                }
                else
                {
                    Debug.LogWarning(gameObject.name + " is from enemy and is not supposed to collide with  " + collision.gameObject.name);
                }
            }
        }

        IEnumerator DestroyGameObject()
        {
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }

        public void TakeDamage(Transform attacker, int damage)
        {
            //Debug.Break();
            float zRotation = 0;
            currentSpeed = parriedSpeed;
            if (isHomingProyectile)
            {
                zRotation = Random.Range(-10, 25);
                target = owner.transform;
            }

            if(owner.transform.position.x - transform.position.x>=0)
            {
                transform.rotation = Quaternion.Euler(0, 0, zRotation);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 180, zRotation);

            }

            owner = attacker.GetComponent<Health>();
            hasHit = false;
            print("New Owner is  " + owner.gameObject.name);
            //Debug.Break();
            //The projectile was parried
            //The projectile will return to the creator
            ///Will be a homing missle
            ///Will be much faster to garanty the hit
            /// Damage the first enemy it encounter
        }
    }
}


