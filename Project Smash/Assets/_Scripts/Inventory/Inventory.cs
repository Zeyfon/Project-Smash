using PSmash.Items;
using PSmash.Stats;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Inventories
{
    public class Inventory : MonoBehaviour
    {
        //CONFIG
        [SerializeField] CraftingSlot[] craftingslot;

        List<Item> inventoryItems = new List<Item>();

        // Start is called before the first frame update
        void Start()
        {
            //Get All items in of the project
            foreach (Item item in Resources.LoadAll<Item>(""))
            {
                inventoryItems.Add(item);
            }
            //Get the actionable items within the previous gotten list
            //foreach(Item item in inventoryItems)
            //{
            //    if(item is ActionableItem)
            //    {
            //        actionableItems.Add(item as ActionableItem);
            //    }
            //}
            //Get the crafting items within the previous gotten list
            //foreach (Item item in inventoryItems)
            //{
            //    if (item is CraftingItem)
            //    {
            //        craftingItems.Add(item as CraftingItem);
            //    }
            //}
            //ReplenishActionableItems();
        }

        private void OnEnable()
        {
            EnemyDrop.onDropCollected += CraftingItemCollected;
        }

        private void CraftingItemCollected(CraftingItem craftingItem)
        {
            foreach(CraftingSlot item in craftingslot)
            {
                if(item.item == craftingItem)
                {
                    item.item.UpdateNumberByThisValue(1);
                    print(item + " was collected");
                }
            }
        }

        public int GetThisCraftingItemNumber(CraftingItem craftingItem) 
        {
            foreach(CraftingSlot slot in craftingslot)
            {
                if(craftingItem == slot.item)
                {
                    return slot.number;
                }
            }
            return 0;
        }


        public void UpdateThisCraftingItem(CraftingItem craftingItem, int number)
        {
            foreach(CraftingSlot slot in craftingslot)
            {
                if(slot.item == craftingItem)
                {
                    slot.number -= number;
                    break;
                }
            }
        }

        public static Inventory GetPlayerInventory()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            return player.GetComponentInChildren<Inventory>();
        }

        [System.Serializable]
        public class CraftingSlot
        {
            public CraftingItem item;
            public int number;
        }

        public void UnlockSkill(Item skill)
        {
            print("In Inventory unlocking the skill  " + skill.name);
            if(skill is ActionableItem)
            {
                GetComponentInParent<Equipment>().UpgradeStock(skill);
            }
            else if(skill is StatusItem)
            {
                GetComponentInParent<BaseStats>().UnlockSkill(skill as StatusItem);
            }
        }
    }

}
