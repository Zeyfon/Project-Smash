using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Inventories
{
    [CreateAssetMenu(menuName = "Items/Crafting Material")]
    public class CraftingItem : Item
    {
        [SerializeField] GameObject gameObject = null;
        [SerializeField] int number = 0;

        public GameObject GetGameObject()
        {
            return gameObject;
        }

        public void UpdateNumberByThisValue( int number)
        {
            this.number+=number;
        }

        public int GetNumber()
        {
            return number;
        }
    }
}
