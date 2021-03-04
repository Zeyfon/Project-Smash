using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Items
{
    /// <summary>
    /// Will be in charge of receiveng the Inputs, and looking for all the items in the Inventory System.
    /// Send to the UseItem the curren equipped item for the player to use it.
    /// Send to the UI the 2 items that will be displayed in the correct order.
    /// </summary>
    public class ItemHandler : MonoBehaviour
    {
        public delegate void ItemChange(InventoryItems.Items[] items);
        public event ItemChange onItemChange;

        int currentIndex = 0;

        // Start is called before the first frame update
        void Start()
        {
            UpdateItems(0);
        }

        public InventoryItems.Items GetEquippedItem()
        {
            InventoryItems.Items[] items = GetComponent<InventoryItems>().GetItems();
            return items[currentIndex];
        }

        public void ChangeItem(bool isMovingRight)
        {
            InventoryItems.Items[] items = GetComponent<InventoryItems>().GetItems();
            print(currentIndex);
            if (isMovingRight)
            {
                currentIndex++;
                print("Moving right" + currentIndex);
                if (currentIndex > items.Length -1)
                {
                    currentIndex = 0;
                }
                print(currentIndex);
            }
            else
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = items.Length - 1;
                }
            }
            UpdateItems(currentIndex);
        }

        internal void ItemUsed(InventoryItems.Items item, int quantity)
        {
            GetComponent<InventoryItems>().UpdateItem(item.item, quantity);
            UpdateUI(currentIndex);
        }

        /// <summary>
        /// Used to call both UseItem.cs and UI Update
        /// </summary>
        /// <param name="items"></param>
        void UpdateItems(int index)
        {
            UpdateUI(index);
        }


        void UpdateUI(int index)
        {
            InventoryItems.Items[] items = GetComponent<InventoryItems>().GetItems();
            InventoryItems.Items[] uIItems = new InventoryItems.Items[2];
            int previousIndex;
            if (index == 0)
            {
                previousIndex = items.Length - 1;
            }
            else
            {
                previousIndex = index - 1;
            }

            uIItems[0] = items[previousIndex];
            uIItems[1] = items[index];

            onItemChange(uIItems);
        }
    }
}

