using PSmash.Attributes;
using PSmash.Combat.Weapons;
using UnityEngine;

namespace PSmash.Items
{
    public class Boulder : MonoBehaviour, IDamagable, IRespawn
    {
        [SerializeField] Weapon interactiveWeapon = null;
        [SerializeField] Rigidbody2D rb;
        [SerializeField] float value = 15f;
        [SerializeField] float applyForcetime = 0.5f;
        [SerializeField] float maxAngularVelocity = 350;
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] AudioClip correctHit = null;
        [SerializeField] AudioClip incorrectHit = null;
        [SerializeField] AudioClip rotatingBoulder = null;
        [SerializeField] AudioClip impactAudio = null;
        [SerializeField] ParticleSystem dust = null;
        [SerializeField] float damage = 100;

        float direction;
        float timer = 0;
        bool isMoving = false;
        float previousAngularVelocity;

        Vector2 initialPosition;

        private void Start()
        {
            initialPosition = transform.position;
        }

        public void TakeDamage(Transform attacker, Weapon weapon, float damage)
        {
            if (weapon == interactiveWeapon)
            {
                direction = GetDirection(attacker);
                audioSource.PlayOneShot(correctHit);
                timer = applyForcetime;
                rb.angularDrag = 0.5f;
            }
            else
            {
                audioSource.PlayOneShot(incorrectHit);
                print("Was not hit with correct weapon");
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            timer -= Time.fixedDeltaTime;

            float angVelocityABS = Mathf.Abs(rb.angularVelocity);
            if (angVelocityABS > maxAngularVelocity)
            {
                float direction = rb.angularVelocity / Mathf.Abs(rb.angularVelocity);
                rb.angularVelocity = maxAngularVelocity * direction;
            }

            bool isMovingAndWasHitByPlayer = angVelocityABS > 50 || timer > 0;
            if (isMovingAndWasHitByPlayer && !audioSource.isPlaying)
            {
                audioSource.loop = true;
                audioSource.clip = rotatingBoulder;
                audioSource.Play();
            }

            bool collidedWithWall = Mathf.Abs(rb.angularVelocity - previousAngularVelocity) > 100;
            if (collidedWithWall)
            {
                print("Collided with wall");
                StopMovement();
                audioSource.PlayOneShot(impactAudio);
            }


            bool isMovingAndIsAlmostStopped = isMoving && angVelocityABS < 50 && timer < 0;
            if (isMovingAndIsAlmostStopped)
            {
                print("Stopped by itself");
                StopMovement();
            }

            previousAngularVelocity = rb.angularVelocity;

            bool isHitByPlayer = timer > 0;
            if (isHitByPlayer)
            {
                Move();
                if (!dust.isPlaying)
                {
                    dust.Play();
                }
            }

        }

        private void Move()
        {
            rb.angularVelocity += (value * direction);
            isMoving = true;
        }

        private void StopMovement()
        {
            isMoving = false;
            rb.angularDrag = 10;
            audioSource.Stop();
            dust.Stop();
        }

        float GetDirection(Transform attacker)
        {
            if (attacker.eulerAngles.y == 180)
                return 1;
            else
                return -1;

        }



        private void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Ground"))
            {
                //print("Contacting with ground");
                dust.transform.position = collision.GetContact(0).point;
                dust.transform.rotation = Quaternion.FromToRotation(new Vector3(0, 0, 0), collision.GetContact(0).normal);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (isMoving && collision.CompareTag("Enemy"))
            {
                collision.GetComponent<IDamagable>().TakeDamage(transform, interactiveWeapon, damage);
            }
        }

        public void Respawn()
        {
            StopMovement();
            transform.position = initialPosition;
        }
    }

}
