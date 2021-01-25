using PSmash.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using PSmash.Stats;

namespace PSmash.Attributes
{
    public class PlayerHealth : Health
    {
        [SerializeField] PlayMakerFSM playerControllerPM;
        [SerializeField] AudioClip damagedSound = null;
        [SerializeField] AudioClip deadSound = null;
        [SerializeField] float timeToRecoverControlAfterDamage = 0.35f;
        public delegate void PlayerisDamaged(float health, float initialHealth);
        public event PlayerisDamaged UpdateUIHealth;

        Coroutine coroutine;
        Animator animator;
        BaseStats baseStats;
        
        bool isDamaged;

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            health = (int)baseStats.GetStat(Stat.Health);
        }

        //Take damage will always be to damage the player
        //The guard and parry states will be checked in the Enemy Attack's Script
        //to deliver the proper assessment

        public override void TakeDamage(Transform attacker, int damage)
        {
            if (isDead) return;
            //print("Damage received");
            isDamaged = true;
            if (coroutine != null) StopCoroutine(coroutine);
            print(damage);
            damage *= (1 - (int)baseStats.GetStat(Stat.Defense)/100);
            print(damage);
            health -= damage;
            playerControllerPM.enabled = false;
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
            
            onTakeDamage.Invoke(damage);
        }

        public float GetMaxHealthPoints()
        {
            return baseStats.GetStat(Stat.Health);
        }

        public float GetHealth()
        {
            return health;
        }

        public void ReplenishHealth(float health)
        {
            this.health += (int)health;
        }

        IEnumerator DamageEffects()
        {
            //print("Playing player damage sound");
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
            yield return new WaitForSeconds(timeToRecoverControlAfterDamage);
            isDamaged = false;
            playerControllerPM.enabled = true;
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


