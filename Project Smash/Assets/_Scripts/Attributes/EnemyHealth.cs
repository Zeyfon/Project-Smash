using HutongGames.PlayMaker;
using PSmash.Combat.Weapons;
using PSmash.Stats;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using PSmash.Combat;
using PSmash.Movement;
using GameDevTV.Saving;
using System.Collections.Generic;
using PSmash.Checkpoints;

namespace PSmash.Attributes
{
    public class EnemyHealth : Health, ISaveable
    {
        [SerializeField] bool isArmorEnabled = false;
        [Header("TestMode")]
        [SerializeField] bool isInvulnerable = true;
        [Header("Extras")]
        [SerializeField] Weapon weaknessWeapon = null;
        [SerializeField] float weaknessFactor = 3;
        [SerializeField] AudioClip protectedDamageAudio = null;
        [SerializeField] AudioClip fleshDamageAudio = null;
        [SerializeField] AudioClip stunnedAudio = null;
        [SerializeField] AudioClip staggerAuduio = null;
        [SerializeField] AudioClip stunEndAudio = null;
        [SerializeField] AudioClip finisherAudio = null;
        [SerializeField] float volume = 1;

        [Header("Finisher")]
        [SerializeField] Vector2 finisherImpulse = new Vector2(15, 7);

        public static event Action onEnemyDead;
        public static List<string> takenOutEnemies = new List<string>();

        EnemyPosture posture;
        PlayMakerFSM pm;
        BaseStats baseStats;
        Vector3 initialPosition;

        float damagePenetrationPercentage;
        bool armorDestroyed = false;

        private void Awake()
        {
            initialPosition = transform.position;
            baseStats = GetComponent<BaseStats>();
            health = baseStats.GetStat(StatsList.Health);
            audioSource = GetComponent<AudioSource>();
            posture = GetComponent<EnemyPosture>();
        }

        void Start()
        {
            if (takenOutEnemies.Contains(GetComponent<SaveableEntity>().GetUniqueIdentifier()))
            {
                transform.parent.gameObject.SetActive(false);
            }
        }

        //This method will always be called from the AIController
        //when entering a new state
        public void SetCurrentStateInfo(PlayMakerFSM pm, float damagePenetrationPercentage)
        {
            this.pm = pm;
            this.damagePenetrationPercentage = damagePenetrationPercentage;
        }

        #region Damaged

        public override void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage)
        {
            if (isDead)
            {
                Debug.LogWarning(gameObject.name + "  is invincible. You can't hurt him anymore");
                return;
            }

            if (pm == null) 
            {
                Debug.LogWarning("No Enemy State set to return DAMAGE Event");
                return;
            }
            Damaged(attacker, weapon, attackType, damage);
        }

        void Damaged(Transform attacker, Weapon attackedWeapon, AttackType attackType, float damage)
        {
            float healthDamage;
            float totalPenetrationPercentage;
            if (weaknessWeapon == attackedWeapon)
            {
                healthDamage = (damage + attackedWeapon.damage) * weaknessFactor ;
                totalPenetrationPercentage = damagePenetrationPercentage + attackedWeapon.damagePenetrationValue * weaknessFactor;
            }
            else
            {
                healthDamage = (damage + attackedWeapon.damage);
                totalPenetrationPercentage = damagePenetrationPercentage + attackedWeapon.damagePenetrationValue;
            }

            //armorDestroyed = false;
            //Unity Event to instantiate a object to show the damage in the screen
            if (posture != null && posture.enabled)
            { 
                float postureDamage;
                if (weaknessWeapon == attackedWeapon)
                {
                    postureDamage = (damage + attackedWeapon.damage) * weaknessFactor;
                }

                else
                {
                    postureDamage = damage + attackedWeapon.damage;
                }
                posture.posture = posture.SubstractDamageFromPosture(postureDamage);
                if (posture.posture <= 0)
                {
                    //print("DAMAGED_STUNNED Event to the fsm " + pm.FsmName);
                    posture.OnStunStateStart();
                    DamageHealth(healthDamage, 100);
                    pm.SendEvent("DAMAGED_STUNNED");
                }
                else
                {

                    FsmEventData myfsmEventData = new FsmEventData();
                    myfsmEventData.FloatData = damage;
                    HutongGames.PlayMaker.Fsm.EventData = myfsmEventData;
                    //print("DAMAGED_NOSTUNNED Event to the fsm " + pm.FsmName);
                    DamageHealth(healthDamage, totalPenetrationPercentage);
                    pm.SendEvent("DAMAGED_NOSTUNNED");
                }
            }
            else
            {
                //print("DAMAGED_NOSTUNNED Event to the fsm " + pm.FsmName);
                DamageHealth(healthDamage, totalPenetrationPercentage);
                pm.SendEvent("DAMAGED_NOSTUNNED");
            }
            if (armorDestroyed)
            {
                TakeArmorOff();
            }
        }

        void DamageHealth(float damage, float damagePenetrationPercentage)
        {
            //print(damage + " will be substracted from  " + health);
            damage *= (1 - baseStats.GetStat(StatsList.Defense) / 100);

            health = SubstractDamageFromHealth(damage, health);
            onTakeDamage.Invoke(damage);
            if (health <= 0)
            {
                GetComponent<AudioSource>().pitch = 1;
                isDead = true;
                takenOutEnemies.Add(GetComponent<SaveableEntity>().GetUniqueIdentifier());
                print("Added to Taken out enemy list " + gameObject.name + "  " + GetComponent<SaveableEntity>().GetUniqueIdentifier());
                if(onEnemyDead != null)
                {
                    onEnemyDead();
                }
            }
        }

        float SubstractDamageFromHealth(float damage, float health)
        {
            if (isInvulnerable) return health;
            health -= damage;
            if (health <= 0) health = 0;
            return health;
        }

        public void Respawn()
        {
            //print(gameObject.name + "  Respawned");
            transform.parent.gameObject.SetActive(true);
            health = baseStats.GetStat(StatsList.Health);
            isDead = false;
            gameObject.layer = LayerMask.NameToLayer("Enemies");
            transform.position = initialPosition;
            if (posture == null)
                return;
            if (!posture.enabled)
            {
                posture.enabled = true;
                PutArmorOn();
            }
            posture.FullyRegenPosture();
        }

        public void PutArmorOn()
        {
            GetComponent<UnblockableAttack>().PutBackArmorOn();
            posture.EnablePostureBar();
        }

        public void TakeArmorOff()
        {
            print("Armor taken off");
            GetComponent<UnblockableAttack>().TakeArmorOff();
            posture.DisablePostureBar();
            ArmoredEnemy armored = GetComponent<ArmoredEnemy>();
            GetComponent<EnemyMovement>().SetSpeedModifier(armored.GetSpeedFactorModifier());
            GetComponent<Animator>().SetFloat("attackSpeed", armored.GetAttackSpeedFactor());
            GetComponent<AudioSource>().pitch = 1.4f;
        }
        #endregion


        #region Finisher
        //Inform the AI that the NPC is being Finished
        public void SetStateToFinisher()
        {
            pm.SendEvent("FINISHER");
        }

        //Start the Finisher Animation once the player and this npc
        // have been correctly positioned for the animation to run
        public void StartFinisherAnimation()
        {
            pm.SendEvent("STARTFINISHERANIMATION");
        }

        //The damage dealt by the player and the thrown away force of the impact
        public void TakeFinisherAttackDamage(Vector3 attackerPosition, float damage)
        {
            audioSource.PlayOneShot(finisherAudio);
            FlyObjectAway(attackerPosition);
            armorDestroyed = true;
            DamageHealth(damage, 100);
            onTakeDamage.Invoke(damage);
        }

        void FlyObjectAway(Vector3 attackerPosition)
        {
            float position = attackerPosition.x - transform.position.x;
            if (position > 0)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(-finisherImpulse.x, finisherImpulse.y);
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = finisherImpulse;
            }
        }

        #endregion

        #region Sounds

        public void ProtectedDamageSound()
        {
            if (GetComponent<ArmoredEnemy>() && !posture.isActiveAndEnabled)
            {
                FleshDamageSound();
                return;
            }
            //print("Protected Audio");
            audioSource.clip = protectedDamageAudio;
            audioSource.Play();
        }

        public void FleshDamageSound()
        {
            //print("Flesh Audio");
            audioSource.clip = fleshDamageAudio;
            audioSource.Play();
        }

        public void StunnedAudio()
        {
            //print("Stunned Audio");
            audioSource.clip = stunnedAudio;
            audioSource.Play();
        }

        public void StaggerAudio()
        {
            //print("Stagger Audio");
            audioSource.clip = staggerAuduio;
            audioSource.Play();
        }

        #endregion

        #region ExternalUse
        public float GetHealthValue()
        {
            return health;
        }

        public float GetMaxHealth()
        {
            return baseStats.GetStat(StatsList.Health);
        }

        public override bool IsDead()
        {
            return isDead;
        }

        public bool IsStunned()
        {
            if(pm.FsmName == "Stun")
            {
                //print("Check if enemy is Stunned  " + fsm.FsmName);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This one is used by the States the entity is.
        /// </summary>
        /// <returns></returns>
        public bool IsDeadCheck()
        {
            return isDead;
        }

        /// <summary>
        /// Used by the Stun State PlayMaker
        /// </summary>
        public void StunEnded()
        {

            audioSource.PlayOneShot(stunEndAudio);
            posture.FullyRegenPosture();
        }

        [System.Serializable]
        struct Info
        {
            public bool isArmorDestroyed;
            public int checkpointCounter;
            public float health;
        }

        public object CaptureState()
        {
            Info info = new Info();
            info.isArmorDestroyed = armorDestroyed;
            info.checkpointCounter = FindObjectOfType<Tent>().GetCheckpointCounter();
            info.health = health;
            return info;
        }

        public void RestoreState(object state)
        {
            //print("Restoring Taken out enemy list " + gameObject.name + "  " + GetComponent<SaveableEntity>().GetUniqueIdentifier());
            if (takenOutEnemies.Contains(GetComponent<SaveableEntity>().GetUniqueIdentifier()))
            {
                transform.parent.gameObject.SetActive(false);
                
            }
            else
            {
                Info info = (Info)state;
                if(info.checkpointCounter == FindObjectOfType<Tent>().GetCheckpointCounter())
                {
                    //print(gameObject.name + "  is in same checkpointNumber");
                    //print("Armor Broken  " + info.isArmorDestroyed);
                    health = info.health;
                    if (Mathf.Approximately(health, 0))
                    {
                        transform.parent.gameObject.SetActive(false);
                    }
                    if (info.isArmorDestroyed)
                    {
                        armorDestroyed = info.isArmorDestroyed;
                        print("Restoring Armor Off");
                        TakeArmorOff();
                    }
                }

            }
        }
        #endregion
    }
}


