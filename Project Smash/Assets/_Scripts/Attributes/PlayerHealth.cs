using PSmash.Analytics;
using PSmash.Combat.Weapons;
using PSmash.Checkpoints;
using PSmash.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using PSmash.Combat;

namespace PSmash.Attributes
{
    public class PlayerHealth : Health
    {

        //CONFIG
        [SerializeField] PlayMakerFSM playerControllerPM;
        [SerializeField] AudioClip damagedSound = null;
        [SerializeField] AudioClip deadSound = null;
        [SerializeField] float timeToRecoverControlAfterDamage = 0.35f;


        //STATE
        public delegate void PlayerDamaged();
        public event PlayerDamaged onDamaged;
        public delegate void PlayerHealed();
        public event PlayerHealed onHealed;
        Coroutine coroutine;
        Animator animator;
        BaseStats baseStats;
        bool isDamaged;

        ////////INITIALIZE///////////

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            audioSource = GetComponent<AudioSource>();
            animator = GetComponent<Animator>();
            health = (int)baseStats.GetStat(StatsList.Health);
        }

        private void OnEnable()
        {
            //print("Enabled");
            Tent.OnTentMenuOpen += RestorePlayer;
        }

        private void OnDisable()
        {
            //print("Disabled");
            Tent.OnTentMenuOpen -= RestorePlayer;
        }

        /////////////////////////////////////////////////////////////////////////////////////////PUBLIC/////////////////////////////////////////////////////////////////////////////////////////////


        /// <summary>
        /// Take the damage done by the enemy or other harmful entities in the game.
        /// Every damage pass through this method. From here the Guard and Parry mechanics are triggered.
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="weapon"></param>
        /// <param name="attackType"></param>
        /// <param name="damage"></param>
        public override void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage)
        {
            if (isDead) 
                return;
            //print("Damage received");
            if (attackType == AttackType.NotUnblockable && GetComponent<PlayerGuard>().IsGuarding(attacker, weapon))
                return;
            isDamaged = true;
            if (coroutine != null) 
                StopCoroutine(coroutine);
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

        public override bool IsDead()
        {
            return isDead;
        }

        public bool IsDamaged()
        {
            return isDamaged;
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

        ///////////////////////////////////////////////////////////////////////////////////////////////////PRIVATE//////////////////////////////////////////////////////////////////

        void RestorePlayer()
        {
            //print(this.gameObject);
            BaseStats baseStats = GetComponent<BaseStats>();
            health = baseStats.GetStat(StatsList.Health);
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
    }
}


