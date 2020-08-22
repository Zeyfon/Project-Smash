using System.Collections;
using System.Threading;
using UnityEngine;

namespace PSmash.Combat
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField] int health = 100;
        [SerializeField] int guard = 100;
        [SerializeField] AudioClip damagedSound = null;
        [SerializeField] AudioClip guardBrokeSound = null;
        [SerializeField] GameObject dropItem = null;
        HealthBar healhtBar;
        GuardBar guardBar;
        Animator animator;
        Coroutine coroutine;
        TestSceneManager sceneManager;
        AudioSource audioSource;
        EnemyMovement movement;
        bool testMode = false;
        bool isDead = false;
        bool isInterrupted = false;
        bool guardBroke = false;
        bool crRunning = false;
        int initialHealth;
        int initialGuard;
        float impulseVelocity = 0;

        // Start is called before the first frame update
        void Start()
        {
            initialHealth = health;
            initialGuard = guard;
            healhtBar = transform.parent.GetComponentInChildren<HealthBar>();
            guardBar = transform.parent.GetComponentInChildren<GuardBar>();
            animator = GetComponent<Animator>();
            sceneManager = FindObjectOfType<TestSceneManager>();
            audioSource = GetComponent<AudioSource>();
            movement = GetComponent<EnemyMovement>();
            if(sceneManager!= null) sceneManager.enemiesAlive.Add(gameObject);
            //For Test Mode
            testMode = GetComponent<EnemyController>().testMode;
        }

        public bool IsDead()
        {
            return isDead;
        }
        public bool IsInterrupted()
        {
            return isInterrupted;
        }

        public void DamageTaken(Transform attacker, int damage)
        {
            if (isDead) return;
            //Debug.Log(gameObject.name + "  was hit");
            if (transform.CompareTag("Enemy") || transform.CompareTag("Player"))
            {
                //Debug.Log(transform.gameObject.name + "  " +  damage);
                Damaged(attacker, damage);
            }
            if (transform.CompareTag("InteractableObject"))
            {
                GetComponent<InteractableObject>().ApplyImpulseForce(attacker.position, damage);
            }
        }

        void Damaged(Transform attacker, int damage)
        {

            if (isDead) return;
            //Debug.Log("Damaged");
            if (guard > 0)
            {
                if (!testMode) guard -= damage;
                if (guard <= 0)
                {
                    guard = 0;
                    guardBroke = true;
                    animator.Play("GuardBroke");
                    isInterrupted = true;
                    impulseVelocity = 10;
                    StunEffects();
                }
                else
                {
                    CheckWhichDamage(attacker);
                }
                float guardScale = (float)guard / (float)initialGuard;
                guardBar.SubstractDamage(guardScale, initialGuard);
            }  
            else
            {
                health -= damage;
                if (health <= 0)
                {
                    health = 0;
                    isDead = true;
                    StartCoroutine(GameObjectDied());
                }
                else
                {
                    CheckWhichDamage(attacker);
                }
                float healthScale = (float)health / (float)initialHealth;
                healhtBar.SubstractDamage(healthScale);
                guardBar.SubstractDamage(0, initialGuard);
            }
            if(attacker != null) movement.ImpulseFromAttack(attacker, impulseVelocity);
        }

        private void CheckWhichDamage(Transform attacker)
        {
            if (attacker == null) animator.Play("DamageOnly", -1, 0f);
            if (!guardBroke && attacker != null)
            {
                animator.Play("Damaged", -1, 0f);
                isInterrupted = true;
                DamageEffects();
                impulseVelocity = 3;
                if (crRunning)
                {
                    crRunning = false;
                    StopCoroutine(coroutine);
                }
                coroutine = StartCoroutine(WaitingForDamageToEnd());
            }
        }

        private void Damaged()
        {
            animator.Play("Damaged", -1, 0f);
            isInterrupted = true;
            DamageEffects();
            if (crRunning)
            {
                crRunning = false;
                StopCoroutine(coroutine);
            }
            coroutine = StartCoroutine(WaitingForDamageToEnd());
        }

        void StunEffects()
        {
            audioSource.PlayOneShot(guardBrokeSound);
        }

        void DamageEffects()
        {
            audioSource.PlayOneShot(damagedSound);
        }

        IEnumerator WaitingForDamageToEnd()
        {
            crRunning = true;
            Debug.Log("Starting to Wait ");
            while (animator.GetInteger("Damaged") != 100)
            {
                yield return new WaitForFixedUpdate();
            }
            isInterrupted = false;
            animator.SetInteger("Damaged", 0);
            crRunning = false;
        }
        IEnumerator GameObjectDied()
        {
            Debug.Log(gameObject.name + " diead");
            animator.Play("Dead", -1, 0f);
            GetComponent<Rigidbody2D>().gravityScale = 0;
            GetComponent<Collider2D>().enabled = false;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            yield return new WaitForSeconds(2);
            if(sceneManager != null) sceneManager.enemiesAlive.Remove(gameObject);
            Destroy(transform.parent.gameObject);
        }
        public void RegeneratingGuard(float guardScale)
        {
            guard = (int)(initialGuard * guardScale);
            //Debug.Log(guard + "  " + initialGuard +  "  " + guardScale);
        }

        void GuardBrokeSound()
        {
            audioSource.PlayOneShot(guardBrokeSound, 1);
        }

        void GuardBrokeRecovery()
        {
            guardBroke = false;
        }
        void DropItem()
        {
            Instantiate(dropItem, transform.position, Quaternion.identity);
        }
    }
}


