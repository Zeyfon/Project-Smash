using PSmash.Items;
using PSmash.Stats;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;

namespace PSmash.Inventories
{
    public class Inventory : MonoBehaviour, ISaveable
    {
        //CONFIG
        [SerializeField] AudioSource collectedDropAudioSource = null;
        [SerializeField] AudioClip collectedDrop = null;
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
            Pickup.onDropCollected += CraftingItemCollected;
        }

        void OnDisable()
        {
            Pickup.onDropCollected -= CraftingItemCollected;
        }

        private void CraftingItemCollected(Pickup.ItemSlot item)
        {
            //print("Crafting item Collected");
            foreach(CraftingSlot slot in slots)
            {
                if(slot.item == item.item as CraftingItem)
                {
                    slot.number ++;
                    collectedDropAudioSource.PlayOneShot(collectedDrop);
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
            //print("In Inventory unlocking the skill  " + skill.name);
            if(skill is SubWeaponItem)
            {
                GetComponentInParent<Equipment>().UpgradeStock(skill);
            }
            else if(skill is StatusItem)
            {
                GetComponentInParent<BaseStats>().UnlockSkill(skill as StatusItem);
            }
        }

        public object CaptureState()
        {
            //print("Inventory being captured");
            Dictionary<string, int> inventoryState = new Dictionary<string, int>();
            foreach (CraftingSlot slot in slots)
            {
                inventoryState.Add(slot.item.GetID(), slot.number);
            }
            return inventoryState;
        }

        public void RestoreState(object state)
        {
            if (ES3.KeyExists("inventory"))
            {
                //print("Inventory being restored");
                Dictionary<string, int> inventoryState = (Dictionary<string, int>)state;
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
