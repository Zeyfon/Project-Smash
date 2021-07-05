using PSmash.Attributes;
using Spine;
using Spine.Unity;
using System.Collections;
using UnityEngine;
using PSmash.Inventories;
using PSmash.Combat;
using PSmash.Stats;

namespace PSmash.Inventories
{
    public class Projectile : UsableItem, IDamagable
    {
        //CONFIG
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
        [SerializeField] float attackForce = 10;

        //STATE
        Transform parabolicMovementTarget;
        bool hasHit = false;
        float timeToDie = 10f;
        float timer = 0;

        ////////////////////////////////////////////////////////INITIALIZE///////////////////////////////////////////////////////////////////
        void Start()
        {
            skeletonAnim.AnimationState.SetAnimation(0, spawn, false);
            skeletonAnim.AnimationState.AddAnimation(0, idleLoop, true, 0.2f);
            skeletonAnim.AnimationState.Complete += OnSpineAnimationEnd;
        }

        void OnDisable()
        {
            skeletonAnim.AnimationState.Complete -= OnSpineAnimationEnd;
        }



        /////////////////////////////////////////////////////////////////////PUBLIC///////////////////////////////////////////////////////
        
        public void Setup()
        {

        }


        /// <summary>
        /// Checks the damage received and use it as a parried attack to change direction and owner
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="weapon"></param>
        /// <param name="attackType"></param>
        /// <param name="damage"></param>
        /// <param name="attackForce"></param>
        public void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage, float attackForce)
        {
            SetObject(true);
            //print("Received the parry "  + hasHit);
            timer = 0;
            skeletonAnim.AnimationState.SetAnimation(0, idleLoop, true);
            float zRotation = Random.Range(-10, 25);
            parabolicMovementTarget = owner.transform;

            if (attacker.position.x - transform.position.x >= 0)

                transform.rotation = Quaternion.Euler(0, 180, zRotation);
            else
                transform.rotation = Quaternion.Euler(0, 0, zRotation);

            owner = attacker.GetComponent<Health>();
            hasHit = false;
        }

        //Anim Event
        void WallHitSound()
        {
            audioSource.volume = 1;
            audioSource.clip = wallHitSound;
            audioSource.pitch = Random.Range(0.8f, 1f);
            audioSource.Play();
        }

        //Anim Event
        void NPCHitSound()
        {
            audioSource.volume = 0.5f;
            audioSource.clip = enemyHitSound;
            audioSource.pitch = Random.Range(0.8f, 1f);
            audioSource.Play();
        }

        /////////////////////////////////////////////////////////////////////////////PRIVATE//////////////////////////////////////////////////////////////////////


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
            //print(hasHit);
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


        //void HitGround(Collider2D collision)
        //{
        //    Vector3 effectsOrigin;
        //    if (GetComponent<BoxCollider2D>())
        //        effectsOrigin = transform.position + new Vector3(GetComponent<BoxCollider2D>().size.x / 2 * transform.right.x, 0);
        //    else
        //        effectsOrigin = transform.position + new Vector3(GetComponent<CircleCollider2D>().radius * transform.right.x, 0);
        //    SetObject(false);
        //    Instantiate(wallHitEWffect, effectsOrigin, Quaternion.identity);
        //    WallHitSound();
        //    SendDamageToIDamagableObject(collision, impactOnWall);
        //}

        void SetObject(bool isEnabled)
        {
            GetComponent<Collider2D>().enabled = isEnabled;
            print("Collider is  " + isEnabled);
            //Debug.Break();
        }

        void SendDamageToIDamagableObject(Collider2D collision, AnimationReferenceAsset anim)
        {
            StopProjectile(anim);
            IDamagable target = collision.GetComponent<IDamagable>();
            if (target != null)
            {
                SetObject(false);
                float damage = owner.GetComponent<BaseStats>().GetStat(StatsList.Attack);
                target.TakeDamage(transform, weapon, AttackType.NotUnblockable, damage, attackForce);
            }
        }

        void StopProjectile(AnimationReferenceAsset anim)
        {
            rb.velocity = new Vector2(0, 0);
            rb.angularVelocity = 0;
            skeletonAnim.AnimationState.SetAnimation(0, anim, false);
            //print("ProjectileStopped");
        }

        void OnSpineAnimationEnd(TrackEntry trackEntry)
        {
            if (trackEntry.Animation.Name == impactOnNPC.name)
                StartCoroutine(Destroy());
        }
        IEnumerator Destroy()
        {
            //print("About to destroy the dagger");
            while (audioSource.isPlaying)
            {
                yield return new WaitForEndOfFrame();
            }
            Destroy(gameObject);
        }

        public void InstantDestroy()
        {
            Destroy(gameObject, 0.1f);
        }

        ////////////////////////////////////////////////////////////////////////COLLISION////////////////////////////////////////////////////////////
        void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Destructible"))
                return;


            //if (collision.CompareTag("Ground"))
            //{
            //    hasHit = true;
            //    HitGround(collision);
            //    return;
            //}

            Health health = collision.GetComponent<Health>();
            if (health == null || health == owner || health.IsDead() || collision.tag == owner.tag)
                return;

            hasHit = true;
            Instantiate(enemyHitEffect, transform.position, Quaternion.identity);
            NPCHitSound();
            SendDamageToIDamagableObject(collision, impactOnNPC);
        }

    }
}


