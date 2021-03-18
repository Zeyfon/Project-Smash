using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using PSmash.Saving;

namespace PSmash.Items
{
    public class Inventory : MonoBehaviour
    {
        //[SerializeField] Items[] items;
        List<Item> inventoryItems = new List<Item>();
        List<ActionableItem> actionableItems = new List<ActionableItem>();
        List<CraftingItem> craftingItems = new List<CraftingItem>();

        public delegate void ItemChange(int index);
        public event ItemChange onEquippedActionItemChange;

        int currentIndex = 0;

        // Start is called before the first frame update
        void Start()
        {
            //Get All items in of the project
            foreach (Item item in Resources.LoadAll<Item>(""))
            {
                inventoryItems.Add(item);
            }
            //Get the actionable items within the previous gotten list
            foreach(Item item in inventoryItems)
            {
                if(item is ActionableItem)
                {
                    actionableItems.Add(item as ActionableItem);
                }
            }
            //Get the crafting items within the previous gotten list
            foreach (Item item in inventoryItems)
            {
                if (item is CraftingItem)
                {
                    craftingItems.Add(item as CraftingItem);
                }
            }
            ReplenishActionableItems();
        }

        private void OnEnable()
        {
            Tent.OnTentMenuOpen += ReplenishActionableItems;
            EnemyDrop.onDropCollected += CraftingItemCollected;
        }

        private void CraftingItemCollected(CraftingItem craftingItem)
        {
            foreach(CraftingItem item in craftingItems)
            {
                if(item == craftingItem)
                {
                    item.UpdateNumberByThisValue(1);
                    print(item + " was collected");
                }
            }
        }

        /// <summary>
        /// Set the quantity of each actionable item back to max
        /// </summary>
        void ReplenishActionableItems()
        {
            //print("Replenishing Items");
            foreach(ActionableItem item in actionableItems)
            {
                item.SetNumber(item.GetMaxNumber());
            }
            UpdateUI(currentIndex);
        }

        public ActionableItem GetEquippedActionableItem()
        {
            return actionableItems[currentIndex];
        }

        public int GetThisCraftingItemNumber(CraftingItem craftingItem) 
        {
            foreach(CraftingItem item in craftingItems)
            {
                if(craftingItem == item)
                {
                    return item.GetNumber();
                }
            }
            return 0;
        }


        public void UpdateThisCraftingItem(CraftingItem craftingItem, int number)
        {
            foreach(CraftingItem item in craftingItems)
            {
                if(item == craftingItem)
                {
                    item.UpdateNumberByThisValue(-number);
                    break;
                }
            }
        }

        public void ChangeItem(bool isMovingRight)
        {
            print(currentIndex);
            if (isMovingRight)
            {
                currentIndex++;
                print("Moving right" + currentIndex);
                if (currentIndex > actionableItems.Count - 1)
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
                    currentIndex = actionableItems.Count - 1;
                }
            }
            UpdateUI(currentIndex);
        }

        public void ItemUsed(ActionableItem item, int quantity)
        {
            UpdateItem(item, quantity);
            UpdateUI(currentIndex);
        }

        /// <summary>
        /// Used to call both UseItem.cs and UI Update
        /// </summary>
        /// <param name="items"></param>
        void UpdateUI(int index)
        {
            onEquippedActionItemChange(index);
        }

        public static Inventory GetPlayerInventory()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            return player.GetComponentInChildren<Inventory>();
        }

        public List<ActionableItem> GetActionableItems()
        {
            return actionableItems;
        }

        public void UpdateItem(ActionableItem item, int quantity)
        {
            foreach(ActionableItem myItem in actionableItems)
            {
                if(myItem != item)
                {
                    continue;
                }
                myItem.SetNumber( quantity);
            }
        }

        [System.Serializable]
        public class Items
        {
            public Item item;
        }
    }

}
