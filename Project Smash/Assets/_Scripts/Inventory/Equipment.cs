using PSmash.Checkpoints;
using UnityEngine;
using GameDevTV.Saving;
using System.Collections.Generic;
using System;
using PSmash.SceneManagement;

namespace PSmash.Inventories
{
    public class Equipment : MonoBehaviour, ISaveable
    {
        [SerializeField] EquipmentSlots[] slots;
        [SerializeField] SubWeaponItem subWeapon;

        public delegate void ToolChange(int index);
        public event ToolChange onToolEquippedUpdate;

        public delegate void SubWeaponChange(SubWeaponItem item);
        public static event SubWeaponChange onSubWeaponChange;

        List<SubWeaponItem> subWeapons = new List<SubWeaponItem>();
        int currentIndex = 0;
        private void Awake()
        {
            GetWeapons();
            RestoreToolsNumber();
        }

        private void GetWeapons()
        {
            foreach (SubWeaponItem item in Resources.LoadAll<SubWeaponItem>(""))
            {
                subWeapons.Add(item);
            }
        }

        void Start()
        {
            onToolEquippedUpdate(currentIndex);
            onSubWeaponChange(subWeapon);
        }

        private void OnEnable()
        {
            Tent.OnCheckpointDone += RestoreToolsNumber2;
        }

        private void OnDisable()
        {
            Tent.OnCheckpointDone -= RestoreToolsNumber2;
        }

        public void SetSubWeapon(SubWeaponItem subWeapon)
        {
            this.subWeapon = subWeapon; 
        }

        public SubWeaponItem GetSubWeapon()
        {
            return subWeapon;
        }

        void RestoreToolsNumber2()
        {
            RestoreToolsNumber();
            onToolEquippedUpdate(currentIndex);
        }
        void RestoreToolsNumber()
        {
            foreach (EquipmentSlots slot in slots)
            {
                slot.number = slot.maxNumber;
            }
        }


        public EquipmentSlots GetEquipmentSlot()
        {
            return slots[currentIndex];
        }

        public void ChangeItem(float movementDirection)
        {
            //print(currentIndex);
            if (movementDirection==1)
            {
                currentIndex++;
                //print("Moving right" + currentIndex);
                if (currentIndex > slots.Length - 1)
                {
                    currentIndex = 0;
                }
                //print(currentIndex);
            }
            else
            {
                currentIndex--;
                if (currentIndex < 0)
                {
                    currentIndex = slots.Length - 1;
                }
            }
            onToolEquippedUpdate(currentIndex);
        }

        public void ItemUsed(EquipmentSlots slot)
        {
            slot.number -= 1;
            onToolEquippedUpdate(currentIndex);
        }

        public EquipmentSlots[] GetTools()
        {
            return slots;
        }


        [System.Serializable]
        public class EquipmentSlots
        {
            public ToolItem item;
            public int number;
            public int maxNumber;
        }

        public void UpgradeStock(Item item)
        {
            foreach(EquipmentSlots slot in slots)
            {
                if(item == slot.item)
                {
                    slot.maxNumber +=1;
                    //print("The " + item.name + "  increase to  " + slot.maxNumber);
                    RestoreToolsNumber();
                    onToolEquippedUpdate(currentIndex);
                    return;
                }
            }

        }

        public object CaptureState()
        {
            ///Save the quantity of each tool
            Dictionary<string, object> toolsForSerialization = new Dictionary<string, object>();
            foreach(EquipmentSlots slot in slots)
            {
                List<int> numbers = new List<int>();
                numbers.Add(slot.number);
                numbers.Add(slot.maxNumber);
                toolsForSerialization.Add(slot.item.GetID(), numbers);
            }

            toolsForSerialization.Add("currentIndex", currentIndex);

            if (subWeapon != null)
            {
                toolsForSerialization.Add("Mace", subWeapon.GetID());
            }
            return toolsForSerialization;
        }

        public void RestoreState(object state, bool isLoadLastScene)
        {
            ///
            var equippedItemsForSerialization = (Dictionary<string, object>)state;

            foreach (string name in equippedItemsForSerialization.Keys)
            {
                var item = (ToolItem)Item.GetFromID(name);
                if (item != null && !isLoadLastScene)
                {
                    foreach(EquipmentSlots slot in slots)
                    {
                        if(item == slot.item)
                        {
                            List<int> tests = (List<int>)equippedItemsForSerialization[name];
                            //print("Restoring " + item.name);
                            slot.number = tests[0];
                            slot.maxNumber = tests[1];
                        }
                    }
                }
                
                if(name == "currentIndex")
                {
                    currentIndex =(int)equippedItemsForSerialization[name];
                    //print("Restoring Tool index  " + currentIndex);
                }

                if(name == "Mace")
                {
                    foreach (SubWeaponItem subWeapon in subWeapons)
                    {
                        string subWeaponName = (string)equippedItemsForSerialization[name];
                        if (subWeaponName == subWeapon.GetID())
                        {
                            this.subWeapon = subWeapon;
                            print("I have the mace");
                        }
                    }
                }
                
            }
            onToolEquippedUpdate(currentIndex);
            onSubWeaponChange(subWeapon);
        }
    }
}

