using PSmash.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

namespace PSmash.Attributes
{
    public class PlayerHealth : Health
    {
        [SerializeField] AudioClip damagedSound = null;
        [SerializeField] AudioClip deadSound = null;
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

        //Take damage will always be to damage the player
        //The guard and parry states will be checked in the Enemy Attack's Script
        //to deliver the proper assessment

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
            print("Playing player damage sound");
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
            AnalyticsEvent.Custom("Player_Died", new Dictionary<string, object>
            {
                { "area_id", LevelProgressionAnalytics.currentSection },
                { "time_elapsed", Time.timeSinceLevelLoad }
            });
            gameObject.layer = LayerMask.NameToLayer("PlayerGhost");
            animator.SetInteger("Damage", 50);
            yield return new WaitForSeconds(2);
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


