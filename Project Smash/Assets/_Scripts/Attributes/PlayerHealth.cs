using PSmash.Analytics;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using PSmash.Stats;
using PSmash.Combat.Weapons;
using PSmash.Saving;

namespace PSmash.Attributes
{
    public class PlayerHealth : Health
    {
        [SerializeField] PlayMakerFSM playerControllerPM;
        [SerializeField] AudioClip damagedSound = null;
        [SerializeField] AudioClip deadSound = null;
        [SerializeField] float timeToRecoverControlAfterDamage = 0.35f;

        public delegate void PlayerDamaged();
        public event PlayerDamaged onDamaged;

        public delegate void PlayerHealed();
        public event PlayerHealed onHealed;



        Coroutine coroutine;
        Animator animator;
        BaseStats baseStats;
        
        bool isDamaged;

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            health = (int)baseStats.GetStat(StatsList.Health);
        }

        private void OnEnable()
        {
            Tent.OnTentMenuOpen += RestorePlayer;
        }


        void RestorePlayer()
        {
            health = GetComponent<BaseStats>().GetStat(StatsList.Health);
            onHealed();
        }
        //Take damage will always be to damage the player
        //The guard and parry states will be checked in the Enemy Attack's Script
        //to deliver the proper assessment

        public override void TakeDamage(Transform attacker, Weapon weapon, float damage)
        {
            if (isDead) return;
            //print("Damage received");
            isDamaged = true;
            if (coroutine != null) StopCoroutine(coroutine);
            damage *= (1 - baseStats.GetStat(StatsList.Defense)/100);
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
            onDamaged();
        }

        public float GetMaxHealthPoints()
        {
            return baseStats.GetStat(StatsList.Health);
        }

        public float GetHealth()
        {
            return health;
        }

        public void RestoreHealth(float health)
        {
            //print("Replenished " + health);
            this.health += health;
            if(this.health> GetMaxHealthPoints())
            {
                //print("Setting health to max health");
                this.health = GetMaxHealthPoints();
            }
            onHealed();
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
        public override bool IsDead()
        {
            return isDead;
        }

        public bool IsDamaged()
        {
            return isDamaged;
        }
    }
}


