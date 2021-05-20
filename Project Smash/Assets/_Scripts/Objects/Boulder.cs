using GameDevTV.Saving;
using PSmash.Attributes;
using PSmash.Checkpoints;
using PSmash.Combat;
using PSmash.Combat.Weapons;
using UnityEngine;

namespace PSmash.Items
{
    public class Boulder : MonoBehaviour, IDamagable, ISaveable
    {

        //CONFIG
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
        [SerializeField] float attackForce = 2;

        //STATE
        float direction;
        float timer = 0;
        bool isMoving = false;
        float previousAngularVelocity;
        int checkpointCounter = 0;


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
                //print("Collided with wall");
                StopMovement();
                audioSource.PlayOneShot(impactAudio);
            }


            bool isMovingAndIsAlmostStopped = isMoving && angVelocityABS < 50 && timer < 0;
            if (isMovingAndIsAlmostStopped)
            {
                //print("Stopped by itself");
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

        ///////////////////////////////////////////////////////////////PUBLIC///////////////////////////////////////////////////////////////
        public void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage, float attackForce)
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
                //print("Was not hit with correct weapon");
            }
        }


        /////////////////////////////////////////////////////////PRIVATE////////////////////////////////////////////////////////////////////

        void Move()
        {
            rb.angularVelocity += (value * direction);
            isMoving = true;
        }

        void StopMovement()
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

        void OnCollisionStay2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Ground"))
            {
                //print("Contacting with ground");
                dust.transform.position = collision.GetContact(0).point;
                dust.transform.rotation = Quaternion.FromToRotation(new Vector3(0, 0, 0), collision.GetContact(0).normal);
            }
        }

        void OnTriggerEnter2D(Collider2D collision)
        {
            if (isMoving && collision.CompareTag("Enemy"))
            {
                collision.GetComponent<IDamagable>().TakeDamage(transform, interactiveWeapon, AttackType.NotUnblockable, damage, attackForce);
            }
        }

        //////////////////////////////////////////////////////SAVING SYSTEM////////////////////////////////////////////////////////

        [System.Serializable]
        struct Info
        {
            public int checkpointCounter;
            public SerializableVector3 position;
        }

        public object CaptureState()
        {
            Info info = new Info();
            info.checkpointCounter = FindObjectOfType<WorldManager>().GetCheckpointCounter();
            info.position = new SerializableVector3(transform.position);
            return info;
        }

        public void RestoreState(object state, bool isLoadLastScene)
        {
            Info info = (Info)state;
            WorldManager worldManager = FindObjectOfType<WorldManager>();
            if (worldManager == null)
            {
                //.LogWarning("Cannot complete the restore state of this entity");
                return;
            }

            checkpointCounter = info.checkpointCounter;
            if (checkpointCounter != worldManager.GetCheckpointCounter())
            {
                //print("No overwrite was applied to  " + gameObject.name);
                return;
            }
            else 
            {
                SerializableVector3 position = info.position;
                Vector3 newPosition = position.ToVector();
                transform.position = newPosition;
            }
        }
    }
}
