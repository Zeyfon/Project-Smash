using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Inventories
{
    [CreateAssetMenu(menuName = "Items/Crafting Material")]
    public class CraftingItem : Item
    {
        [SerializeField] GameObject gameObject = null;

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
