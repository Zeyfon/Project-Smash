using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using PSmash.Combat;

namespace PSmash.Resources
{
    public class PlayerHealth : Health
    {
        [SerializeField] AudioClip damagedSound = null;
        public delegate void PlayerisDamaged(float health, float initialHealth);
        public event PlayerisDamaged OnPlayerDamage;

        Coroutine coroutine;
        Animator animator;

        bool isDamaged;

        private void Awake()
        {
            animator = GetComponent<Animator>();
        }

        // Start is called before the first frame update
        void Start()
        {
            initialHealth = health;
        }

        public override void TakeDamage(Transform attacker, int damage)
        {
            if (isDead) return;
            print("Damage received");
            isDamaged = true;
            if (coroutine != null) StopCoroutine(coroutine);
            health -= damage;
            if (health <= 0)
            {
                health = 0;
                isDead = true;
                StartCoroutine(EntityDied());
            }
            else
            {
                coroutine = StartCoroutine(DamageEffects());
                StartCoroutine(ControlReset());
            }
            OnPlayerDamage(health, initialHealth);
        }

        IEnumerator DamageEffects()
        {
            GetComponent<AudioSource>().PlayOneShot(damagedSound);
            animator.SetInteger("Damage", 1);
            while(animator.GetInteger("Damage") != 100)
            {
                yield return new WaitForEndOfFrame();
            }
            animator.SetInteger("Damage", 0);
            coroutine = null;
        }

        IEnumerator ControlReset()
        {
            yield return new WaitForSeconds(0.11f);
            isDamaged = false;
        }

        IEnumerator EntityDied()
        {
            animator.SetInteger("Damage", 50);
            yield return new WaitForSeconds(2);
            //Destroy(gameObject);
            SceneManager.LoadScene(0);
        }

        public bool IsDead()
        {
            return isDead;
        }

        public bool IsDamaged()
        {
            return isDamaged;
        }
    }
}


