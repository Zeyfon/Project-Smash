using System.Collections;
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
        bool isDead = false;
        bool isGuardActive = true;
        bool isInterruptedByDamage = false;
        bool isBreakingGuard = false;
        bool crRunning = false;
        int initialHealth;
        int initialGuard;
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
            if(sceneManager!= null) sceneManager.enemiesAlive.Add(gameObject);
        }

        public bool IsDead()
        {
            return isDead;
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

            //Debug.Log("Damaged");
            float guardScale;
            float healthScale;
            if (guard > 0)
            {
                guard -= damage;
                if (guard <= 0)
                {
                    guard = 0;
                    isBreakingGuard = true;
                    Debug.Log("Stunned");
                    animator.SetInteger("Damaged", 20);
                    isInterruptedByDamage = true;
                    if (!crRunning)
                    {
                        Debug.Log("Starting Coroutine");
                        StartCoroutine(WaitingForDamageToEnd());
                        //Debug.Break();
                    }
                }
                guardScale = (float)guard / (float)initialGuard;
                guardBar.SubstractDamage(guardScale, initialGuard);
            }  
            else
            {
                health -= damage;
                if (health <= 0)
                {
                    StopAllCoroutines();
                    health = 0;
                    isDead = true;
                    Debug.Log(gameObject.name + " diead");
                    StartCoroutine(GameObjectDied());
                }
                healthScale = (float)health / (float)initialHealth;
                healhtBar.SubstractDamage(healthScale);
                guardBar.SubstractDamage(0, initialGuard);
            }
            if (isDead) return;
            if (attacker == null)
            {
                Debug.Log("Damage Only");
                //Debug.Break();
                animator.SetTrigger("DamageOnly");
            }

            if (attacker != null && !isBreakingGuard)
            {
                audioSource.PlayOneShot(damagedSound);
                //Debug.Log("Interrupted  " + attacker.gameObject);
                animator.SetInteger("Damaged", 1);
                isInterruptedByDamage = true;
                if (!crRunning)
                {
                    Debug.Log("Coroutine has finished");
                    coroutine = StartCoroutine(WaitingForDamageToEnd());
                }
            }

        }

        IEnumerator WaitingForDamageToEnd()
        {
            crRunning = true;
            Debug.Log("Starting to Wait ");
            while (animator.GetInteger("Damaged") != 100)
            {
                yield return new WaitForFixedUpdate();
            }
            isInterruptedByDamage = false;
            isBreakingGuard = false;
            animator.SetInteger("Damaged", 0);
            //Debug.Break();
            crRunning = false;
        }
        public bool IsBeingInterruptedByDamage()
        {
            return isInterruptedByDamage;
        }
        IEnumerator GameObjectDied()
        {
            animator.SetInteger("Damaged", 90);
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

        void DropItem()
        {
            Instantiate(dropItem, transform.position, Quaternion.identity);
        }
    }
}


