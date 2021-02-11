﻿using HutongGames.PlayMaker;
using PSmash.Stats;
using System.Collections;
using UnityEngine;

namespace PSmash.Attributes
{
    public class EnemyHealth : Health
    {
        [Header("TestMode")]
        [SerializeField] bool isInvulnerable = true;
        [Header("Extras")]
        [SerializeField] GameObject dropItem = null;

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
            //print(fsm.FsmName + "  set in " + this);
            this.pm = pm;
            this.damagePenetrationPercentage = damagePenetrationPercentage;
        }

        public override void TakeDamage(Transform attacker, WeaponList weapon, float damage)
        {
            if (isDead) 
                return;
            if (pm == null) 
            {
                Debug.LogWarning("No Enemy State set to return DAMAGE Event");
                return;
            }
            Damaged(damage);
        }

        public void Damaged(float damage)
        {
            //Unity Event to instantiate a object to show the damage in the screen
            onTakeDamage.Invoke(damage);

            if(posture != null)
            {
                posture.posture = posture.SubstractDamageFromPosture(damage);
                if (posture.posture <= 0)
                {
                    print("POSTUREDEPLETED Event to the fsm " + pm.FsmName);
                    posture.OnStunStateStart();
                    DamageHealth(damage, 100);
                    pm.SendEvent("DAMAGED_POSTUREDEPLETED");
                    return;
                }
                else
                {

                    FsmEventData myfsmEventData = new FsmEventData();
                    myfsmEventData.FloatData = damage;
                    HutongGames.PlayMaker.Fsm.EventData = myfsmEventData;
                    print("DAMAGED_POSTURENOTDEPLETED Event to the fsm " + pm.FsmName);
                    DamageHealth(damage, damagePenetrationPercentage);
                    pm.SendEvent("DAMAGED_POSTURENOTDEPLETED");
                    return;
                }
            }
            else
            {
                print("DAMAGED_NOPOSTUREBAR Event to the fsm " + pm.FsmName);
                DamageHealth(damage, damagePenetrationPercentage);
                pm.SendEvent("DAMAGED_NOPOSTUREBAR");
                return;
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
            print(damage + " will be substracted from  " + health);
            damage = damage * damagePenetrationPercentage / 100;
            health = SubstractDamageFromHealth(damage, health);
            if (health <= 0)
            {
                //print("Dead");
                isDead = true;
            }
                        print(gameObject.name + "  current health is : " + health );
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
            DamageHealth(damage, 100);
            onTakeDamage.Invoke(damage);
            yield return null;
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


