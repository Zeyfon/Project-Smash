using PSmash.Checkpoints;
using UnityEngine;
using GameDevTV.Saving;
using GameDevTV.Saving;
using System.Collections.Generic;

namespace PSmash.Inventories
{
    public class Equipment : MonoBehaviour, ISaveable
    {
        [SerializeField] EquipmentSlots[] slots;
        [SerializeField] SubWeaponItem subWeapon;

        public delegate void ItemChange(int index);
        public event ItemChange onEquipmentUIUpdate;

        int currentIndex = 0;
        private void Awake()
        {
            RestoreToolsNumber();
        }

        private void OnEnable()
        {
            Tent.OnTentMenuOpen += RestoreToolsNumber;
        }

        private void OnDisable()
        {
            Tent.OnTentMenuOpen -= RestoreToolsNumber;
        }

        public void SetSubWeapon(SubWeaponItem subWeapon)
        {
            this.subWeapon = subWeapon; 
        }

        public SubWeaponItem GetSubWeapon()
        {
            return subWeapon;
        }

        void RestoreToolsNumber()
        {
            foreach (EquipmentSlots slot in slots)
            {
                slot.number = slot.maxNumber;
            }
            onEquipmentUIUpdate(currentIndex);
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
            onEquipmentUIUpdate(currentIndex);
        }

        public void ItemUsed(EquipmentSlots slot)
        {
            slot.number -= 1;
            onEquipmentUIUpdate(currentIndex);
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
                    print("The " + item.name + "  increase to  " + slot.maxNumber);
                    RestoreToolsNumber();
                    return;
                }
            }

        }

        public object CaptureState()
        {
            ///Save the quantity of each tool
            Dictionary<string, List<int>> toolsForSerialization = new Dictionary<string, List<int>>();
            foreach(EquipmentSlots slot in slots)
            {
                List<int> numbers = new List<int>();
                numbers.Add(slot.number);
                numbers.Add(slot.maxNumber);
                toolsForSerialization.Add(slot.item.GetID(), numbers);
            }
            List<int> currentIndexes = new List<int>();
            currentIndexes.Add(currentIndex);

            toolsForSerialization.Add("currentIndex", currentIndexes); 
            return toolsForSerialization;
        }

        public void RestoreState(object state)
        {
            ///
            var equippedItemsForSerialization = (Dictionary<string, List<int>>)state;

            foreach (string name in equippedItemsForSerialization.Keys)
            {
                var item = (ToolItem)Item.GetFromID(name);
                if (item != null)
                {
                    foreach(EquipmentSlots slot in slots)
                    {
                        if(item == slot.item)
                        {
                            print("Restoring " + item.name);
                            slot.number = equippedItemsForSerialization[name][0];
                            slot.maxNumber = equippedItemsForSerialization[name][1];
                        }
                    }
                }
                if(name == "currentIndex")
                {
                    currentIndex = equippedItemsForSerialization[name][0];
                    print("Restoring Tool index  " + currentIndex);
                }
            }
            onEquipmentUIUpdate(currentIndex);
        }
    }
}

