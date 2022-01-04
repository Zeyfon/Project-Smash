using PSmash.Items;
using PSmash.Stats;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using PSmash.CraftingSystem;

namespace PSmash.Inventories
{
    public class Inventory : MonoBehaviour, ISaveable
    {
        //CONFIG
        [SerializeField] AudioSource collectedDropAudioSource = null;
        [SerializeField] AudioClip collectedDrop = null;
        [SerializeField] List<CraftingSlot> craftingItemSlots = new List<CraftingSlot>();


        List<Item> inventoryItems = new List<Item>();
        List<Weapon> weapons = new List<Weapon>();
        List<CollectibleItem> collectibles = new List<CollectibleItem>();
        Dictionary<string, float> extraDamageForWeapons = new Dictionary<string, float>();

        // Start is called before the first frame update
        void Awake()
        {
            foreach (Item item in Resources.LoadAll<Item>(""))
            {
                inventoryItems.Add(item);
            }

            foreach (Item item in inventoryItems)
            {
                if (item is Weapon)
                {
                    Weapon weapon = item as Weapon;
                    weapons.Add(weapon);
                    //print(item.displayName);
                    extraDamageForWeapons.Add(weapon.GetID(), weapon.GetDamage());
                }
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

        public List<CraftingSlot> GetCraftingItemsList()
        {
            return craftingItemSlots;
        }

        public List<Weapon> GetWeapons()
        {
            return weapons;
        }


        void CraftingItemCollected(Pickup.ItemSlot item)
        {
            //print("Crafting item Collected");
            foreach(CraftingSlot slot in craftingItemSlots)
            {
                if(slot.item == item.item as CraftingItem)
                {
                    slot.number += item.number;
                    collectedDropAudioSource.PlayOneShot(collectedDrop);
                }
            }
        }

        public Weapon GetMainWeapon()
        {
            foreach (Weapon weapon in weapons)
            {
                if (weapon.GetID() == "22fb0e72-60f1-4908-81f3-ed22b0195c88")
                    return weapon;
            }
            Debug.LogError("No Main Weapon Found");
            return null;
        }

        public Subweapon GetSubweapon()
        {
            foreach(Weapon weapon in weapons)
            {
                if(weapon is Subweapon)
                {
                    return weapon as Subweapon;
                }
            }
            return null;
        }

        public int GetThisCraftingItemNumber(CraftingItem craftingItem) 
        {
            foreach(CraftingSlot slot in craftingItemSlots)
            {
                if(craftingItem == slot.item)
                {
                    return slot.number;
                }
            }
            return 0;
        }

        public void SubstractTheseCraftingItemsNumbers(SkillSlot.CraftingItemSlot[] slots)
        {
            foreach (SkillSlot.CraftingItemSlot slot in slots)
            {
                foreach(CraftingSlot item in craftingItemSlots)

                if(slot.item == item.item)
                {
                        item.number -= slot.number;
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
            if(skill is ToolItem)
            {
                GetComponentInParent<Equipment>().UpgradeStock(skill);
            }
            else if(skill is Weapon)
            {
                UpgradeWeaponDamage(skill);
            }
        }

        void UpgradeWeaponDamage(Item item)
        {
            foreach (string weaponID in extraDamageForWeapons.Keys)
            {
                if (weaponID == item.GetID())
                {
                    Item weapon = Item.GetFromID(weaponID);
                    float weaponDamage = Mathf.Round((weapon as Weapon).GetDamage() * 0.25f);
                    float currentWeaponDamage = extraDamageForWeapons[weaponID];
                    extraDamageForWeapons[weaponID] = currentWeaponDamage + weaponDamage;
                    print(weaponID + "  " + extraDamageForWeapons[weaponID]);
                    break;
                }
            }
        }

        public float GetExtraWeaponDamage(Weapon equippedWeapon)
        {
            foreach (string weaponID in extraDamageForWeapons.Keys)
            {
                if (weaponID == equippedWeapon.GetID())
                {
                    return extraDamageForWeapons[weaponID];
                }
            }
            Debug.LogWarning("Weapon not found for extra damage");
            return 0;
        }

        public List<CollectibleItem> GetCollectibles()
        {
            return collectibles;
        }

        public void AddCollectibleToInventory(CollectibleItem item)
        {
            collectibles.Add(item);
        }

        ////////////////////////////////////////////////SAVE SYSTEM///////////////////////////////////////////////////////////////

        public object CaptureState()
        {
            //print("Inventory being captured");
            Dictionary<string, object> inventoryState = new Dictionary<string, object>();
            Dictionary<string, int> craftingItemsState = new Dictionary<string, int>();
            foreach (CraftingSlot slot in craftingItemSlots)
            {
                craftingItemsState.Add(slot.item.GetID(), slot.number);
                if (slot.number != 0)
                {
                    //print(slot.item.GetDisplayName() + "  was captured with  " + slot.number);
                }
            }
            inventoryState.Add("CraftingItems", craftingItemsState);
            //print(extraDamageForWeapons.Count + "   counting");

            inventoryState.Add("ExtraDamageWeapons", extraDamageForWeapons);
            List<string> collectiblesID = new List<string>();
            int i = 0;
            foreach(CollectibleItem collectible in collectibles)
            {
                collectiblesID.Add(collectible.GetID());
            }
            inventoryState.Add("Collectibles", collectiblesID);
            return inventoryState;
        }

        public void RestoreState(object state, bool isLoadLastScene)
        {
            Dictionary<string, object> inventoryState = (Dictionary<string, object>)state;
            foreach (string name in inventoryState.Keys)
            {
                if (name == "CraftingItems")
                {
                    Dictionary<string, int> craftingSlotsSaved = (Dictionary<string, int>)inventoryState[name];
                    //Dictionary<string, int> craftingItemsState = new Dictionary<string, int>();
                    foreach (string itemID in craftingSlotsSaved.Keys)
                    {
                        foreach (CraftingSlot slot in craftingItemSlots)
                        {
                            
                            if (slot.item.GetID() == itemID)
                            {
                                slot.number = craftingSlotsSaved[itemID];
                                if (slot.number != 0)
                                {
                                    //print("Restoring  " + slot.item.GetDisplayName() + "  with  " + slot.number);
                                }
                            }
                        }
                    }
                }
                else if (name == "ExtraDamageWeapons")
                {
                    Dictionary<string, float> extraDamage = inventoryState[name] as Dictionary<string, float>;
                    extraDamageForWeapons.Clear();
                    extraDamageForWeapons = extraDamage;
                    foreach(string weaponID in extraDamage.Keys)
                    {
                        //print(Item.GetFromID(weaponID) + " has now  " + extraDamage[weaponID]);
                    }
                }
                else if( name == "Collectibles")
                {
                    List<string> collectiblesID = (List<string>)inventoryState[name];
                    foreach(string collectiblID in collectiblesID)
                    {
                        collectibles.Add(Item.GetFromID(collectiblID) as CollectibleItem);
                    }
                }
            }
        }
    }
}
