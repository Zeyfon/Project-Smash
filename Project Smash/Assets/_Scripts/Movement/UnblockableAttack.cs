﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

namespace PSmash.Movement
{
    public class UnblockableAttack : MonoBehaviour
    {

        [Header("Special Attack Condition")]
        [SerializeField] float specialAttackRange;
        [SerializeField] float distanceCheckForObstacles;
        [SerializeField] LayerMask whatIsGround;
        [SerializeField] LayerMask whatIsEnemy;


        [Header("Special Attack")]
        [Tooltip("This factor will multiply the base speed the entity has")]
        [SerializeField] float specialAttackSpeedFactor;

        Transform playerTransform;

        [Header("Color Tint")]
        [SerializeField] Material defaultMaterial = null;
        [SerializeField] Material redTintMaterial = null;
        [SerializeField] Material addedMaterial = null;
        [SerializeField] float fadeIntTime = 0.5f;

        private void Awake()
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            if (addedMaterial != null)
            {
                GetComponent<SkeletonRenderer>().CustomMaterialOverride.Add(defaultMaterial, addedMaterial);
                //defaultMaterial = addedMaterial;
            }
        }


        #region Special Attack Conditions
        /// <summary>
        /// This part will be in charge of checking specific conditions for this attack to take place
        /// Besides the other conditions placed in the Chase State.
        /// It is called by a state fsm inside Playmaker of the current gameObject
        /// </summary>
        /// <param name="pm"></param>
        public void CanDoSpecialMovement(PlayMakerFSM pm)
        {
            if (IsEnemyBetweenPlayerAndMe() && IsTargetInSpecialAttackRange())
            {
                print("Sending to State " + pm.FsmName + " SPECIAL ATTACK Event ");
                if (pm == null)
                {
                    return;
                }
                else
                {
                    print("SpecialAttack coming from " + transform.parent.gameObject.name);
                    //Debug.Break();
                    pm.SendEvent("SPECIALATTACK");
                    return;
                }

            }
        }
        private bool IsEnemyBetweenPlayerAndMe()
        {
            //print("Looking for enemy in front");
            Debug.DrawRay(transform.position + new Vector3(0, 1, 0), transform.right, Color.cyan);
            RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, 1), transform.right, distanceCheckForObstacles, whatIsEnemy);
            if (hit && (hit.collider.gameObject != gameObject))
            {
                print(hit.collider.gameObject.tag);
                //print("There is an obstacle between the target and me");
                return true;
            }
            else
            {
                //print("There is no obstacle between the target and me");
                return false;
            }
        }

        private bool IsTargetInSpecialAttackRange()
        {
            //print(specialAttackRange +"  "+ Vector3.Distance(transform.position, targetPosition));
            return specialAttackRange > Vector3.Distance(transform.position, playerTransform.position);
        }

        #endregion

        #region Special Attack
        /// <summary>
        /// This region of the script will have everything related to the performance of the attack
        /// </summary>
        //AnimEvent 
        void StartSpecialAttackImpulse()
        {
            print("StartSpecial Attack Impulse");

            //GetComponent<EnemyMovement>().SpecialAttackImpulse_Start(specialAttackSpeedFactor);
        }

        //This method can be called via Anim Event and via SpecialAttack State in the PlayMaker
        public void StopSpecialAttackImpulse()
        {
            print("Stop Special Attack Impulse");
            GetComponent<EnemyMovement>().SpecialAttackImpulse_Stop();
            ReturnToOriginalTint();
            //Debug.Break();
        }
        //Called from the Special Attack FSM

        public void TintMaterialToRed()
        {
            SkeletonRenderer skeletonRenderer = GetComponent<SkeletonRenderer>();
            if (addedMaterial != null)
            {
                skeletonRenderer.CustomMaterialOverride.Remove(defaultMaterial);
            }
            print("Tinting red");
            skeletonRenderer.CustomMaterialOverride.Add(defaultMaterial, redTintMaterial);
        }

        public void ReturnToOriginalTint()
        {
            SkeletonRenderer skeletonRenderer = GetComponent<SkeletonRenderer>();
            skeletonRenderer.CustomMaterialOverride.Remove(defaultMaterial);
            if (addedMaterial != null)
            {
                skeletonRenderer.CustomMaterialOverride.Add(defaultMaterial, addedMaterial);
            }
            gameObject.layer = LayerMask.NameToLayer("Enemies");
        }

        IEnumerator FadeIn(Material currentMaterial)
        {
            Color tintColor = currentMaterial.GetColor("_Tint");
            float alpha = 0;
            while (alpha != 1)
            {
                alpha += Time.deltaTime / fadeIntTime;
                if (alpha >= 1) alpha = 1;

                currentMaterial.SetColor("_Tint", new Color(tintColor.r, tintColor.g, tintColor.b, alpha));
                //print(currentMaterial.GetColor("_Tint"));
                yield return null;
            }
        }

        IEnumerator FadeOut(Material currentMaterial)
        {
            Color tintColor = currentMaterial.GetColor("_Tint");
            float alpha = 0;
            while (alpha != 1)
            {
                alpha += Time.deltaTime / fadeIntTime;
                if (alpha >= 1) alpha = 1;

                currentMaterial.SetColor("_Tint", new Color(tintColor.r, tintColor.g, tintColor.b, alpha));
                //print(currentMaterial.GetColor("_Tint"));
                yield return null;
            }
        }

        #endregion

    }

}