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

        AudioSource audioSource;
        Rigidbody2D rb;
        Health target;
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

        //Used by the Attack FSM of the Ranger01
        public void SetTarget(Health target)
        {
            this.target = target;
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
            string tag;
            if (target == null)
                tag = "Enemy";
            else
                tag = "Player";

            print(tag);
            if (collision.CompareTag(tag))
            {
                hasHit = true;
                Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
                collision.GetComponent<IDamagable>().TakeDamage(transform, damage);
                if (GetComponent<Animator>() != null)
                    GetComponent<Animator>().SetTrigger("TargetHit");
                NPCHitSound();
                StartCoroutine(DestroyGameObject());
            }
            else if(collision.CompareTag("Ground"))
            {
                hasHit = true;
                Instantiate(wallHitEWffect, transform.position + new Vector3(GetComponent<BoxCollider2D>().size.x/2*transform.right.x,0), Quaternion.identity);
                if (GetComponent<Animator>() != null)
                    GetComponent<Animator>().SetTrigger("WallHit");
                WallHitSound();
                StartCoroutine(DestroyGameObject());
            }
            else
            {
                Debug.LogWarning(gameObject.name + " did not know to what it collided");
            }
        }

        IEnumerator DestroyGameObject()
        {
            yield return new WaitForSeconds(2);
            Destroy(gameObject);
        }

        public void TakeDamage(Transform attacker, int damage)
        {
            //The projectile was parried
            //The projectile will return to the creator
            ///Will be a homing missle
            ///Will be much faster to garanty the hit
            /// Damage the first enemy it encounter
        }
    }
}


