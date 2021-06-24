using GameDevTV.Saving;
using PSmash.Checkpoints;
using PSmash.Combat;
using PSmash.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{
    public class ArmoredEnemy : MonoBehaviour, ISaveable
    {
        [SerializeField] float unarmoredMovementSpeedModifier;
        [SerializeField] float unarmoredAttackSpeedModifier;

        bool isArmorEnable = true;

        static int checkpointCounter = 0;

        public float GetSpeedFactorModifier()
        {
            return unarmoredMovementSpeedModifier;
        }

        public float GetAttackSpeedModifier()
        {
            return unarmoredAttackSpeedModifier;
        }

        /// <summary>
        /// Sets the enemy without armor. Used in the Restored State 
        /// and as an Anim Event in the FnisherAttackDamaged Animation
        /// </summary>
        public void TakeArmorOff()
        {
            print("Armor taken off");
            GetComponent<UnblockableAttack>().TakeArmorOffSpineSkins();
            GetComponent<EnemyPosture>().DisablePostureBar();
            ArmoredEnemy armored = GetComponent<ArmoredEnemy>();
            GetComponent<EnemyMovement>().SetSpeedMovementModifierValue(armored.GetSpeedFactorModifier(), armored.GetAttackSpeedModifier());
            GetComponent<AudioSource>().pitch = 1.4f;
            isArmorEnable = false;
        }

        [System.Serializable]
        struct Info
        {
            public int checkpointCounter;
            public bool isArmorEnable;
        }

        public object CaptureState()
        {
            Info info = new Info();
            info.checkpointCounter = FindObjectOfType<WorldManager>().GetCheckpointCounter();
            return info;
        }

        public void RestoreState(object state, bool isLoadLastScene)
        {
            WorldManager worldManager = FindObjectOfType<WorldManager>();
            Info info = (Info)state;
            checkpointCounter = info.checkpointCounter;
            if (checkpointCounter != worldManager.GetCheckpointCounter())
            {
                print("No overwrite was applied to  " + gameObject.name);
                return;
            }
            print("Enemy without armor");
            isArmorEnable = info.isArmorEnable;
            if (!isArmorEnable)
            {
                print("Enemy without armor");
                TakeArmorOff();
            }

        }
    }

}
