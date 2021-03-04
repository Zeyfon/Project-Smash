using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PSmash.Items
{
    public class InventoryItems : MonoBehaviour
    {
        [SerializeField] Items[] items;

        public Items[] GetItems()
        {
            return items;
        }

        public void UpdateItem(Item item, int quantity)
        {
            foreach(Items myItem in items)
            {
                if(myItem.item != item)
                {
                    continue;
                }
                myItem.quantity = quantity;
            }
        }

        [System.Serializable]
        public class Items
        {
            public Item item;
            public int quantity;
        }
    }

}
