﻿using PSmash.Items;
using PSmash.Stats;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Saving;

namespace PSmash.Inventories
{
    public class Inventory : MonoBehaviour, ISaveable
    {
        //CONFIG
        [SerializeField] CraftingSlot[] slots;

        List<Item> inventoryItems = new List<Item>();

        // Start is called before the first frame update
        void Start()
        {
            //Get All items in of the project
            foreach (Item item in Resources.LoadAll<Item>(""))
            {
                inventoryItems.Add(item);
            }
        }

        private void OnEnable()
        {
            EnemyDrop.onDropCollected += CraftingItemCollected;
        }

        void OnDisable()
        {
            EnemyDrop.onDropCollected -= CraftingItemCollected;
        }

        private void CraftingItemCollected(CraftingItem craftingItem)
        {
            print("Crafting item Collected");
            foreach(CraftingSlot slot in slots)
            {
                if(slot.item == craftingItem)
                {
                    slot.number ++;
                    print(slot + " was collected");
                }
            }
        }

        public int GetThisCraftingItemNumber(CraftingItem craftingItem) 
        {
            foreach(CraftingSlot slot in slots)
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
            foreach(CraftingSlot slot in slots)
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

        public void CaptureState()
        {
            print("Inventory being captured");
            Dictionary<string, int> inventoryState = new Dictionary<string, int>();
            foreach (CraftingSlot slot in slots)
            {
                inventoryState.Add(slot.item.GetID(), slot.number);
            }

            ES3.Save("inventory", inventoryState);
        }

        public void RestoreState()
        {
            if (ES3.KeyExists("inventory"))
            {
                print("Inventory being restored");
                Dictionary<string, int> inventoryState = (Dictionary<string, int>)ES3.Load("inventory");
                foreach (string itemName in inventoryState.Keys)
                {
                    foreach (CraftingSlot slot in slots)
                    {
                        if (slot.item.GetID() == itemName)
                        {
                            slot.number = inventoryState[itemName];
                        }
                    }
                }
            }
        }
    }
}