using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace PSmash.LevelUpSystem
{
    [CreateAssetMenu(fileName = "New Crafting Material", menuName = "CraftingMaterials")]
    public class CraftingMaterial : ScriptableObject
    {
        public CraftingMaterialsList material;
        public Sprite sprite;
        public string description;
    }
}

