using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Items
{
    [CreateAssetMenu(menuName = "Items/Crafting Material")]
    public class CraftingItem : Item
    {
        [SerializeField] int number = 0;


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
