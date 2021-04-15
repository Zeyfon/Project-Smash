using PSmash.Attributes;
using Spine;
using Spine.Unity;
using System.Collections;
using UnityEngine;
using PSmash.Combat.Weapons;
using PSmash.Combat;
using PSmash.Stats;

namespace PSmash.Inventories
{
    public class Projectile : UsableItem, IDamagable
    {
        [Header("General")]
        [SerializeField] Rigidbody2D rb = null;
        [SerializeField] AudioSource audioSource = null;

        [Header("Independent Object Values")]
        [Tooltip("The initial speed at which it will start moving")]
        [SerializeField] float speed = 1;
        [Tooltip("The rotation speed that will have for the angular velocity in the RigidBody. Only works when the projectile is parried")]
        [SerializeField] float rotateSpeed = 5;
        [Tooltip("The factor at which the speed will increase once parried")]
        [SerializeField] float parriedSpeedFactor = 3;
        [SerializeField] GameObject enemyHitEffect = null;
        [SerializeField] GameObject wallHitEWffect = null;
        [SerializeField] AudioClip wallHitSound = null;
        [SerializeField] AudioClip enemyHitSound = null;

        [Header("Spine")]
        [SerializeField] SkeletonAnimation skeletonAnim = null;
        [SerializeField] AnimationReferenceAsset spawn = null;
        [SerializeField] AnimationReferenceAsset idleLoop = null;
        [SerializeField] AnimationReferenceAsset impactOnNPC = null;
        [SerializeField] AnimationReferenceAsset impactOnWall = null;

        [Header("Weapon")]
        [SerializeField] Weapon weapon;

        //Health owner;
        Transform parabolicMovementTarget;
        bool hasHit = false;
        float timeToDie = 10f;
        float timer = 0;

        /// <summary>
        /// This scripts controls the projectile movement as the animations from spine
        /// In The start method the events from the Skeleton Animations are subscribed
        /// </summary>
        void Start()
        {
            skeletonAnim.AnimationState.SetAnimation(0, spawn, false);
            skeletonAnim.AnimationState.AddAnimation(0, idleLoop, true, 0.2f);
            skeletonAnim.AnimationState.Complete += OnSpineAnimationEnd;
        }

        void Update()
        {
            timer += Time.deltaTime;
            if (timer > timeToDie)
            {
                Destroy(gameObject);
            }
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (hasHit)
                return;

            //print("Moving");
            if (parabolicMovementTarget != null)
            {
                //print("Rotating towards " + target.gameObject.name);
                Vector2 direction = (Vector2)parabolicMovementTarget.position - rb.position;
                direction.Normalize();
                float rotateAmount = Vector3.Cross(direction, transform.right).z;
                rb.angularVelocity = -rotateAmount * rotateSpeed;
            }
            rb.velocity = transform.right * speed;
        }

        void OnDisable()
        {
            skeletonAnim.AnimationState.Complete -= OnSpineAnimationEnd;
        }


        public void SetWeapon(Weapon weapon)
        {
            this.weapon = weapon;
        }

        public void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage)
        {
            timer = 0;
            print("Received the parry");
            skeletonAnim.AnimationState.SetAnimation(0, idleLoop, true);
            float zRotation = Random.Range(-10, 25);
            //The owner right now is the enemy, so it is set as the new target
            //and after all the set the owner will be updated to be the player
            parabolicMovementTarget = owner.transform;

            if (owner.transform.position.x - transform.position.x >= 0)
                transform.rotation = Quaternion.Euler(0, 0, zRotation);
            else
                transform.rotation = Quaternion.Euler(0, 180, zRotation);

            owner = attacker.GetComponent<Health>();
            hasHit = false;
        }


        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Destructible"))
                return;
            if (hasHit)
                return;

            if (owner == null)
            {
                print("Hit test");
                HitGround(collision);
                return;
            }



            string tag = owner.gameObject.tag;
            //Collision with the playe being the current owner of the projectile
            if (tag == "Player")
            {

                if (collision.CompareTag("Enemy"))
                {
                    if (collision.GetComponent<Health>().IsDead())
                        return;
                    hasHit = true;
                    print("Attack from Player colliding with enemy");
                    Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
                    NPCHitSound();
                    ProjectileCollisionImpact(collision, impactOnNPC);
                }
                else /*if (collision.CompareTag("Ground"))*/
                {
                    //Debug.Break();
                    hasHit = true;
                    print("Attack from player colliding with ground");
                    HitGround(collision);
                }
            }
            //Collision with the enemy being the current owner of the projectile
            else
            {

                if (collision.CompareTag("Player"))
                {
                    if (collision.GetComponent<Health>().IsDead())
                        return;
                    hasHit = true;
                    Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
                    NPCHitSound();
                    print("Attack from enemy colliding with player");
                    ProjectileCollisionImpact(collision, impactOnNPC);
                }
                else if (collision.GetComponent<PlayerGuard>())
                {
                    hasHit = true;
                    print("Attack from enemy colliding with guard");
                    Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
                    ProjectileCollisionImpact(collision, impactOnNPC);
                }
                else if (collision.CompareTag("Ground"))
                {
                    //Debug.Break();
                    hasHit = true;
                    print("Attack from enemy colliding with ground");
                    HitGround(collision);
                }
            }
        }

        private void HitGround(Collider2D collision)
        {
            Vector3 effectsOrigin;
            if (GetComponent<BoxCollider2D>())
                effectsOrigin = transform.position + new Vector3(GetComponent<BoxCollider2D>().size.x / 2 * transform.right.x, 0);
            else
                effectsOrigin = transform.position + new Vector3(GetComponent<CircleCollider2D>().radius * transform.right.x, 0);
            Instantiate(wallHitEWffect, effectsOrigin, Quaternion.identity);
            WallHitSound();
            ProjectileCollisionImpact(collision, impactOnWall);
        }

        private void ProjectileCollisionImpact(Collider2D collision, AnimationReferenceAsset anim)
        {
            rb.velocity = new Vector2(0, 0);
            rb.angularVelocity = 0;
            skeletonAnim.AnimationState.SetAnimation(0, anim, false);
            IDamagable target = collision.GetComponent<IDamagable>();
            if (target != null)
            {
                float damage = owner.GetComponent<BaseStats>().GetStat(StatsList.Attack);
                target.TakeDamage(transform, weapon, AttackType.NotUnblockable, damage);
            }
        }

        private void OnSpineAnimationEnd(TrackEntry trackEntry)
        {
            if (trackEntry.Animation.Name == impactOnNPC.name)
                StartCoroutine(Destroy());
        }
        IEnumerator Destroy()
        {
            while (audioSource.isPlaying)
            {
                yield return new WaitForEndOfFrame();
            }
            Destroy(gameObject);
        }

        //Anim Event
        void WallHitSound()
        {
            audioSource.clip = wallHitSound;
            audioSource.pitch = Random.Range(0.8f, 1f);
            audioSource.Play();
        }

        //Anim Event
        void NPCHitSound()
        {
            audioSource.clip = enemyHitSound;
            audioSource.pitch = Random.Range(0.8f, 1f);
            audioSource.Play();
        }

    }
}


