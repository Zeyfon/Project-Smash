using PSmash.Attributes;
using PSmash.Movement;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;

namespace PSmash.Combat
{
    public class EnemyFighter : MonoBehaviour
    {
        [SerializeField] Material defaultMaterial = null;
        [SerializeField] Material unblockableMaterial = null;
        [SerializeField] Material addedMaterial = null;
        [SerializeField] float fadeIntTime = 0.5f;
        [SerializeField] Vector2 unblockableAttackImpulse;
        [SerializeField] PlayMakerFSM aiControllerpm = null;
        [SerializeField] float aggroDistance = 2;
        [SerializeField] LayerMask whatIsEnemy;


        EnemyMovement movement;

        bool canAggrevateNearbyEnemies = false;

        // Start is called before the first frame update
        void Awake()
        {
            movement = GetComponent<EnemyMovement>();
            if (addedMaterial != null)
            {
                GetComponent<SkeletonRenderer>().CustomMaterialOverride.Add(defaultMaterial, addedMaterial);
                //defaultMaterial = addedMaterial;
            }

        }

        void FixedUpdate()
        {
            if (canAggrevateNearbyEnemies)
            {
                //print("LookingToAggrevateNearbyEnemies");
                AggrevateNearbyEnemies();
            }
        }

        public void SetAggrevateNearbyEnemies(bool state)
        {
            canAggrevateNearbyEnemies = state;
        }
        void AggrevateNearbyEnemies()
        {
            Debug.DrawRay(transform.position + new Vector3(0, 1, 0), transform.right, Color.blue);

            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + new Vector3(0, 1),transform.right,aggroDistance,whatIsEnemy);
            if (hits.Length == 0)
            {
                //print("No Enemies in front to aggrevate");
            }
            foreach(RaycastHit2D hit in hits)
            {
                //print("Aggrevating  this one  " + hit.collider.gameObject.name);
                hit.collider.GetComponent<EnemyFighter>().Aggrevate();
            }
        }

        public void Aggrevate()
        {
            aiControllerpm.SendEvent("AGGREVATED");
        }
        public void ChangeColor()
        {
            SkeletonRenderer skeletonRenderer = GetComponent<SkeletonRenderer>();
            if (addedMaterial != null)
            {
                skeletonRenderer.CustomMaterialOverride.Remove(defaultMaterial);
            }
            skeletonRenderer.CustomMaterialOverride.Add(defaultMaterial, unblockableMaterial);
        }

        public void ReturnOriginalColor()
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

        //AnimEvent
        void UnblockableAttackImpulse()
        {
            print("Special Attack Force Application");
            gameObject.layer = LayerMask.NameToLayer("EnemiesGhost");
            movement.Impulse(unblockableAttackImpulse);
        }

    }
}


