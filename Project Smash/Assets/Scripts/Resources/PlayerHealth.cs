using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using PSmash.Combat;

namespace PSmash.Resources
{
    public class PlayerHealth : Health
    {
        [SerializeField] AudioClip damagedSound = null;
        [SerializeField] AudioClip deadSound = null;
        public delegate void PlayerisDamaged(float health, float initialHealth);
        public event PlayerisDamaged OnPlayerDamage;

        Coroutine coroutine;
        Animator animator;
        PlayerFighter fighter;

        bool isDamaged;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            fighter = GetComponent<PlayerFighter>();
        }

        // Start is called before the first frame update
        void Start()
        {
            initialHealth = health;
        }

        public override void TakeDamage(Transform attacker, int damage)
        {
            if (isDead) return;
            if (fighter.IsFinishingAnEnemy()) return;
            //print("Damage received");
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
            print("Playing player damage sound");
            animator.SetInteger("Damage", 1);
            while(animator.GetInteger("Damage") != 100)
            {
                yield return new WaitForEndOfFrame();
            }
            animator.SetInteger("Damage", 0);
            if (GetComponent<PlayerFighter>().IsGuardButtonPressed()) GetComponent<PlayerFighter>().Guard();
            coroutine = null;
        }

        IEnumerator ControlReset()
        {
            yield return new WaitForSeconds(0.11f);
            isDamaged = false;
        }

        IEnumerator EntityDied()
        {
            gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
            animator.SetInteger("Damage", 50);
            yield return new WaitForSeconds(2);
            //Destroy(gameObject);
            SceneManager.LoadScene(0);
        }

        //AnimEvent
        void DamageSound()
        {
            audioSource.PlayOneShot(damagedSound);

        }
        //AnimEvent
        void DeadSound()
        {
            audioSource.PlayOneShot(deadSound);
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


