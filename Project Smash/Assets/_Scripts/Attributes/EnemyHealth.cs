using HutongGames.PlayMaker;
using PSmash.Combat.Weapons;
using PSmash.Stats;
using System;
using System.Collections;
using UnityEngine;

namespace PSmash.Attributes
{
    public class EnemyHealth : Health
    {
        [SerializeField] bool isArmorEnabled = false;
        [Header("TestMode")]
        [SerializeField] bool isInvulnerable = true;
        [Header("Extras")]
        [SerializeField] GameObject dropItem = null;
        [SerializeField] Weapon weaknessWeapon = null;
        [SerializeField] float weaknessFactor = 3;
        [SerializeField] AudioClip protectedDamageAudio = null;
        [SerializeField] AudioClip fleshDamageAudio = null;
        [SerializeField] AudioClip stunnedAudio = null;
        [SerializeField] AudioClip staggerAuduio = null;
        [SerializeField] float volume = 1;

        EnemyPosture posture;
        PlayMakerFSM pm;
        BaseStats baseStats;

        float damagePenetrationPercentage;

        private void Awake()
        {
            baseStats = GetComponent<BaseStats>();
            health = baseStats.GetStat(StatsList.Health);
            audioSource = GetComponent<AudioSource>();
            posture = GetComponent<EnemyPosture>();
        }

        //This method will always be called from the AIController
        //when entering a new state
        public void SetCurrentStateInfo(PlayMakerFSM pm, float damagePenetrationPercentage)
        {
            print(gameObject.name + " set in " + pm.FsmName);
            this.pm = pm;
            this.damagePenetrationPercentage = damagePenetrationPercentage;
        }

        public override void TakeDamage(Transform attacker, Weapon weapon, float damage)
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
            Damaged(weapon, damage);
        }

        public void Damaged(Weapon attackedWeapon, float damage)
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

            bool disablePostureBar = false;
            //Unity Event to instantiate a object to show the damage in the screen
            if (posture != null && posture.enabled)
            { 
                float postureDamage;
                if (weaknessWeapon == attackedWeapon)
                {
                    postureDamage = (damage + attackedWeapon.damage) * weaknessFactor;
                    if(posture.posture < postureDamage)
                    {
                        disablePostureBar = true;
                    }
                }

                else
                {
                    postureDamage = damage + attackedWeapon.damage;
                }
                posture.posture = posture.SubstractDamageFromPosture(postureDamage);
                if (posture.posture <= 0)
                {
                    print("DAMAGED_STUNNED Event to the fsm " + pm.FsmName);
                    posture.OnStunStateStart();
                    DamageHealth(healthDamage, 100);
                    pm.SendEvent("DAMAGED_STUNNED");
                }
                else
                {

                    FsmEventData myfsmEventData = new FsmEventData();
                    myfsmEventData.FloatData = damage;
                    HutongGames.PlayMaker.Fsm.EventData = myfsmEventData;
                    print("DAMAGED_NOSTUNNED Event to the fsm " + pm.FsmName);
                    DamageHealth(healthDamage, totalPenetrationPercentage);
                    pm.SendEvent("DAMAGED_NOSTUNNED");
                }
            }
            else
            {
                print("DAMAGED_NOSTUNNED Event to the fsm " + pm.FsmName);
                DamageHealth(healthDamage, totalPenetrationPercentage);
                pm.SendEvent("DAMAGED_NOSTUNNED");
            }
            if (disablePostureBar)
            {
                DisablePostureBar();
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

        public void DamageHealth(float damage, float damagePenetrationPercentage)
        {
            //print(damage + " will be substracted from  " + health);
            //damage = damage * damagePenetrationPercentage / 100;
            damage *= (1 - baseStats.GetStat(StatsList.Defense) / 100);

            health = SubstractDamageFromHealth(damage, health);
            onTakeDamage.Invoke(damage);
            if (health <= 0)
            {
                //print("Dead");
                isDead = true;
            }
        }

        private float SubstractDamageFromHealth(float damage, float health)
        {
            if (isInvulnerable) return health;
            health -= damage;
            if (health <= 0) health = 0;
            return health;
        }


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
        public void DeliverFinishingBlow(Vector3 attackerPosition, float damage)
        {
            StartCoroutine(FinisherReaction(attackerPosition, damage));
        }

        public IEnumerator FinisherReaction(Vector3 attackerPosition, float damage)
        {
            float x = 15;
            float y = 7;
            float position = attackerPosition.x - transform.position.x;
            GetComponent<Rigidbody2D>().gravityScale = 1;
            GetComponent<Rigidbody2D>().drag = 2.5f;
            if (position > 0)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(-x, y);
            }
            else
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(x, y);
            }
            if (isArmorEnabled)
            {
                DisablePostureBar();
            }
            DamageHealth(damage, 100);
            onTakeDamage.Invoke(damage);
            yield return null;
        }

        private void DisablePostureBar()
        {
            posture.DisablePostureBar();
            GetComponent<BaseStats>().SetStat(StatsList.Defense, 0);
        }

        public void ProtectedDamageSound()
        {
            if (isArmorEnabled && !posture.isActiveAndEnabled)
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

        #endregion

        //Anim Event
        void DropItem()
        {
            Instantiate(dropItem, transform.position + new Vector3(0,1), Quaternion.identity);
        }

        //AnimEvent
        void Destroy()
        {
            Destroy(transform.parent.gameObject);
        }
    }
}


