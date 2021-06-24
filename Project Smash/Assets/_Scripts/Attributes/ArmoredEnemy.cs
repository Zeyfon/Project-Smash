using GameDevTV.Saving;
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

        public object CaptureState()
        {
            return isArmorEnable;
        }

        public void RestoreState(object state, bool isLoadLastScene)
        {
            print("Enemy without armor");
            isArmorEnable = (bool)state;
            if (!isArmorEnable)
            {
                print("Enemy without armor");
                TakeArmorOff();
            }

        }
    }

}
