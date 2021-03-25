using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Inventories;

namespace PSmash.CraftingSystem
{
    [CreateAssetMenu(menuName = "Skills/ Skill")]

    public class Skill : ScriptableObject
    {
        [SerializeField] Item item = null;
        [SerializeField] Sprite sprite = null;
        [SerializeField] [TextArea] string description = null;
        
        public Item GetItem()
        {
            return item;
        }
         
        public Sprite GetSprite()
        {
            return sprite;
        }

        public string GetDescription()
        {
            return description;
        }
    }

}
