using HutongGames.PlayMaker;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Spine;
using Spine.Unity;

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

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            posture = GetComponent<EnemyPosture>();
        }

        // Start is called before the first frame update
        void Start()
        {
            initialHealth = health;
        }

        //This method will always be called from the AIController
        //when entering a new state
        public void SetCurrentState(PlayMakerFSM pm)
        {
            //print(fsm.FsmName + "  set in " + this);
            this.pm = pm;
        }

        public override void TakeDamage(Transform attacker, int damage)
        {
            if (isDead) 
                return;
            if (pm == null) 
            {
                Debug.LogWarning("No Enemy State set to return DAMAGE Event");
                return;
            }
            print("Will Send DAMAGED Event with " + damage + " of damage to " + pm.FsmName + " State");
            FsmEventData myfsmEventData = new FsmEventData();
            myfsmEventData.IntData = damage;
            myfsmEventData.GameObjectData = gameObject;
            HutongGames.PlayMaker.Fsm.EventData = myfsmEventData;
            print(pm.FsmName);
            if(pm != null)
                pm.Fsm.Event("DAMAGED");
        }

        public void Damaged(int damage, int damagePenetrationPercentage)
        {
            print(gameObject.name + "  Damaged");
            if(posture != null)
            {
                posture.posture = posture.SubstractDamageFromPosture(damage);
                if (posture.posture <= 0 && pm.FsmName != "Stun")
                {
                    print("POSTUREDEPLETED Event to the fsm " + pm.FsmName);
                    posture.OnStunStateStart();
                    DamageHealth(damage, 100);
                    pm.SendEvent("POSTUREDEPLETED");
                    return;
                }
                else
                {
                    print("CONTINUE Event to the fsm " + pm.FsmName);
                    DamageHealth(damage, damagePenetrationPercentage);
                    pm.SendEvent("CONTINUE");
                    return;
                }
            }
            else
            {
                print("CONTINUE Event to the fsm " + pm.FsmName);
                DamageHealth(damage, damagePenetrationPercentage);
                pm.SendEvent("CONTINUE");
                return;
            }
        }

        public void IsDeadCheck()
        {
            if (isDead)
            {
                //print(pm.FsmName + "  DEAD Event");
                pm.SendEvent("ISDEAD");
            }
            else
            {
                //print(pm.FsmName + "  DEAD Event");
                pm.SendEvent("ISNOTDEAD");
            }
        }

        public void DamageHealth(int damage, int damagePenetrationPercentage)
        {
            //print(damage + " being substract from Health");
            damage = damage * damagePenetrationPercentage / 100;
            health = SubstractDamageFromHealth(damage, health);
            if (health <= 0)
            {
                //print("Dead");
                isDead = true;
            }
        }

        private int SubstractDamageFromHealth(int damage, int health)
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
        public void DeliverFinishingBlow(Vector3 attackerPosition, int damage)
        {
            StartCoroutine(FinisherReaction(attackerPosition, damage));
        }

        public IEnumerator FinisherReaction(Vector3 attackerPosition, int damage)
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
            yield return null;
        }


        #region ExternalUse
        public float GetHealthValue()
        {
            return health;
        }

        public float GetInitialHealthValue()
        {
            return initialHealth;
        }

        public bool IsDead()
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


