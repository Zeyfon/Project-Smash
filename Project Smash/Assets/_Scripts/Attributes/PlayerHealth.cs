using PSmash.Analytics;
using PSmash.Inventories;
using PSmash.Checkpoints;
using PSmash.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using PSmash.Combat;
using GameDevTV.Saving;
using PSmash.SceneManagement;
using PSmash.Movement;

namespace PSmash.Attributes
{
    public class PlayerHealth : Health, ISaveable
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
            InitializeHealth();

        }

        private void OnEnable()
        {
            Tent.OnCheckpointDone += RestorePlayer;
        }

        private void OnDisable()
        {
            Tent.OnCheckpointDone -= RestorePlayer;
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
        public override void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage, float attackForce)
        {
            //TODO
            //I cannot find the Knockack Movement applied to Merle when attacked
            //The knockback movement must be applied using the attackForce value received in the Method
            //Right now the movement is constant and do not use the variable

            print("player damagedd " + damage);
            if (isDead) 
                return;
            if (attackType == AttackType.NotUnblockable && GetComponent<PlayerGuard>().IsGuarding(attacker, weapon))
                return;
            isDamaged = true;
            if (coroutine != null) 
                StopCoroutine(coroutine);
            damage *= (1 - baseStats.GetStat(StatsList.Defense)/100);
            health -= damage;
            print("Player Health " + health);
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

            GetComponent<PlayerMovement>().ApplyAttackImpactReceived(attacker, weapon, LayerMask.NameToLayer("PlayerGhost"),LayerMask.NameToLayer("Player"));

            DamageSlot slot = new DamageSlot();
            slot.damage = damage;
            slot.damageType = DamageType.Health;
            slot.criticalType = CriticalType.NoCritical;
            onTakeDamage.Invoke(slot);
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
        void InitializeHealth()
        {
            if (health == 0)
            {
                //print("Health Initialized from 0  " + gameObject.name);
                health = baseStats.GetStat(StatsList.Health);
            }
        }

        void RestorePlayer()
        {
            //print(this.gameObject);
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
            //FindObjectOfType<WorldManager>().IncreaseCheckpointCounter();
            FindObjectOfType<SavingWrapper>().LoadLastSavedScene();
        }

        public object CaptureState()
        {
            Dictionary<string, float> healthState = new Dictionary<string, float>();
            healthState.Add("health", health);
            return healthState;
        }

        public void RestoreState(object state, bool isLoadLastScene)
        {
            if (isLoadLastScene)
                return;


            Dictionary<string, float> healthstate = (Dictionary<string, float>)state;
            foreach(string name in healthstate.Keys)
            {
                if(name == "health")
                {
                    health = healthstate[name];
                    onHealed();
                    break;
                }
                print("health from file to restores was not found");
            }
        }
    }
}


