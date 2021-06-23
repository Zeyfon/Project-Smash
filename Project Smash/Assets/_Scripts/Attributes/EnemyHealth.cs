using GameDevTV.Saving;
using HutongGames.PlayMaker;
using PSmash.Checkpoints;
using PSmash.Combat;
using PSmash.Inventories;
using PSmash.Movement;
using PSmash.Stats;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{
    public class EnemyHealth : Health, ISaveable
    {

        //CONFIG
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
        [SerializeField] float damageMitigator = 0;

        [Header("Finisher")]
        [SerializeField] Vector2 finisherImpulse = new Vector2(15, 7);

        public static event Action onEnemyDead;
        public static List<string> takenOutEnemies = new List<string>();


        //STATE
        EnemyPosture posture;
        PlayMakerFSM pm;
        BaseStats baseStats;
        bool armorDestroyed = false;

        static int checkpointCounter = 0;

        //INITIALIZE
        private void Awake()
        {
            if (!GetComponent<EnemyPosture>())
                GetComponentInChildren<PostureBar>().gameObject.SetActive(false);
            baseStats = GetComponent<BaseStats>();
            health = baseStats.GetStat(StatsList.Health);
            audioSource = GetComponent<AudioSource>();
            posture = GetComponent<EnemyPosture>();
        }

        /////////////////////////////////////////////////////////////PUBLIC////////////////////////////////////////
        //This method will always be called from the AIController
        //when entering a new state
        public void SetCurrentStateInfo(PlayMakerFSM pm, float damagePenetrationPercentage)
        {
            this.pm = pm;
        }

        public override void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float characterDamage, float attackForce)
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
            Damaged(weapon, characterDamage);
            ///
            GetComponent<EnemyMovement>().ApplyAttackImpactReceived(attacker, weapon, LayerMask.NameToLayer("EnemiesGhost"), LayerMask.NameToLayer("Enemies"));
        }

        //This is done every time the player hits the enemy after the armor was off
        //TODO 
        //Fix this issue. This must be used only once

        //Inform the AI that the NPC is being Finished
        public void SetStateToFinisher()
        {
            pm.SendEvent("FINISHER");
        }

        //Start the Finisher Animation once the player and this npc
        // have been correctly positioned for the animation to run
        public void StartFinisherAnimation()
        {
            //Debug.Break();
            pm.SendEvent("STARTFINISHERANIMATION");
        }

        //The damage dealt by the player and the thrown away force of the impact
        public void TakeFinisherAttackDamage(Vector3 attackerPosition, float damage)
        {
            print("Taking Finisher Damage");
            audioSource.PlayOneShot(finisherAudio);
            FlyObjectAway(attackerPosition);
            DamageHealth(damage);
            if(isArmorEnabled)
                armorDestroyed = true;
        }

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
            if (pm.FsmName == "Stun")
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

        #endregion

        ////////////////////////////////////////////////////////PRIVATE///////////////////////////////////////////
        
        /// <summary>
        /// Set the damage that will be applied to the entity
        /// The final damage will be a combination of the base player damage, the weapon damage and several modifiers
        /// </summary>
        /// <param name="attacker"></param>
        /// <param name="attackedWeapon"></param>
        /// <param name="attackType"></param>
        /// <param name="characterDamage"></param>
        void Damaged(Weapon attackedWeapon, float characterDamage)
        {
            print("Damaged with " + attackedWeapon + "  " + "characterDamage "   + characterDamage + "  weaponDamage  " + attackedWeapon.GetDamage());

            float healthDamage = (characterDamage + attackedWeapon.GetDamage());
            //armorDestroyed = false;
            //Unity Event to instantiate a object to show the damage in the screen
            if (posture != null && posture.enabled)
            {
                //TODO MITIGATOR FACTOR WHEN POSTURE ENABLED

                float postureDamage = characterDamage + attackedWeapon.GetDamage();
                healthDamage -= baseStats.GetStat(StatsList.Defense);
                if (healthDamage <= 0)
                    healthDamage = 0;
                Debug.Log("Damage after Mitigation is " + healthDamage);
                posture.posture = posture.SubstractDamageFromPosture(postureDamage);
                OnDamageTaken(DamageType.Posture, postureDamage);
                if (posture.posture <= 0)
                {
                    //print("1_DAMAGED_STUNNED Event to the fsm " + pm.FsmName);
                    posture.OnStunStateStart();
                    DamageHealth(healthDamage);
                    pm.SendEvent("DAMAGED_STUNNED");
                }
                else
                {

                    FsmEventData myfsmEventData = new FsmEventData();
                    myfsmEventData.FloatData = characterDamage;
                    HutongGames.PlayMaker.Fsm.EventData = myfsmEventData;
                    //print("2_DAMAGED_NOSTUNNED Event to the fsm " + pm.FsmName);
                    DamageHealth(healthDamage);
                    pm.SendEvent("DAMAGED_NOSTUNNED");
                }
            }
            else
            {
                Debug.Log("Full Damage" + healthDamage);
                //print("3_DAMAGED_NOSTUNNED Event to the fsm " + pm.FsmName);
                DamageHealth(healthDamage);
                pm.SendEvent("DAMAGED_NOSTUNNED");
            }
            print("My Current health  " + health);
        }

        void OnDamageTaken(DamageType damageType, float damage)
        {
            DamageSlot slot = new DamageSlot();
            slot.damage = damage;
            slot.damageType = damageType;
            onTakeDamage.Invoke(slot);
        }

        void DamageHealth(float damage)
        {
            //print(damage + " will be substracted from  " + health);
            //damage *= (1 - (baseStats.GetStat(StatsList.Defense)) / 100);

            health = SubstractDamageFromHealth(damage, health);
            OnDamageTaken(DamageType.Health, damage);
            if (health <= 0)
            {
                GetComponent<AudioSource>().pitch = 1;
                isDead = true;
                onDead.Invoke();
            }
        }

        float SubstractDamageFromHealth(float damage, float health)
        {
            if (isInvulnerable) return health;
            health -= damage;
            if (health <= 0) health = 0;
            return health;
        }

        /// <summary>
        /// Sets the enemy without armor. Used in the Restored State 
        /// and as an Anim Event in the FnisherAttackDamaged Animation
        /// </summary>
        void TakeArmorOff()
        {
            //print("Armor taken off");
            GetComponent<UnblockableAttack>().TakeArmorOffSpineSkins();
            //armorModifier = baseStats.GetStat(StatsList.Defense);
            posture.DisablePostureBar();
            damageMitigator = 0;
            ArmoredEnemy armored = GetComponent<ArmoredEnemy>();
            GetComponent<EnemyMovement>().SetSpeedMovementModifierValue(armored.GetSpeedFactorModifier(),armored.GetAttackSpeedModifier());
            GetComponent<AudioSource>().pitch = 1.4f;
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

        //////////////////////////////////////////////SAVE SYSTEM///////////////////////////////////////

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
            info.checkpointCounter = FindObjectOfType<WorldManager>().GetCheckpointCounter();
            //print(gameObject.name + "  was saved with the Checkpoint Counter of  " + info.checkpointCounter);
            info.health = health;
            return info;
        }

        public void RestoreState(object state, bool isLoadLastScene)
        {
            //Will restore everytime the save system ask for it
            //Depending on the counter in the WorldManager it will overwrite the data or not
            Info info = (Info)state;
            WorldManager worldManager = FindObjectOfType<WorldManager>();
            if (worldManager == null)
            {
                //Debug.LogWarning("Cannot complete the restore state of this entity");
                return;
            }

            checkpointCounter = info.checkpointCounter;
            if (checkpointCounter != worldManager.GetCheckpointCounter())
            {
                print("No overwrite was applied to  " + gameObject.name);
                return;
            }
            else
            {
                health = info.health;
                if (Mathf.Approximately(health, 0))
                    DisableEnemy();
                else if (info.isArmorDestroyed)
                {
                    armorDestroyed = info.isArmorDestroyed;
                    //print("Restoring Armor Off");
                    TakeArmorOff();
                }
            }
        }

        void DisableEnemy()
        {

            foreach (PlayMakerFSM pm in GetComponents<PlayMakerFSM>())
            {
                pm.enabled = false;
            }
            Animator anim = GetComponent<Animator>();
            if (anim != null)
                anim.enabled = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            gameObject.layer = LayerMask.NameToLayer("Dead");
        }
    }

}


