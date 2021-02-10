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

        [SerializeField] LayerMask whatIsEnemy;

        // Start is called before the first frame update
        void Awake()
        {
            if (addedMaterial != null)
            {
                GetComponent<SkeletonRenderer>().CustomMaterialOverride.Add(defaultMaterial, addedMaterial);
                //defaultMaterial = addedMaterial;
            }
        }

        //public void ChangeColor()
        //{ 
        //    print("ChangingColor");
        //    SkeletonRenderer skeletonRenderer = GetComponent<SkeletonRenderer>();
        //    if (addedMaterial != null)
        //    {
        //        skeletonRenderer.CustomMaterialOverride.Remove(defaultMaterial);
        //    }
        //    skeletonRenderer.CustomMaterialOverride.Add(defaultMaterial, unblockableMaterial);
        //}

        //public void ReturnOriginalColor()
        //{
        //    SkeletonRenderer skeletonRenderer = GetComponent<SkeletonRenderer>();
        //    skeletonRenderer.CustomMaterialOverride.Remove(defaultMaterial);
        //    if (addedMaterial != null)
        //    {
        //        skeletonRenderer.CustomMaterialOverride.Add(defaultMaterial, addedMaterial);
        //    }
        //    gameObject.layer = LayerMask.NameToLayer("Enemies");
        //}

        //IEnumerator FadeIn(Material currentMaterial)
        //{
        //    Color tintColor = currentMaterial.GetColor("_Tint");
        //    float alpha = 0;
        //    while (alpha != 1)
        //    {
        //        alpha += Time.deltaTime / fadeIntTime;
        //        if (alpha >= 1) alpha = 1;

        //        currentMaterial.SetColor("_Tint", new Color(tintColor.r, tintColor.g, tintColor.b, alpha));
        //        //print(currentMaterial.GetColor("_Tint"));
        //        yield return null;
        //    }
        //}

    }
}


