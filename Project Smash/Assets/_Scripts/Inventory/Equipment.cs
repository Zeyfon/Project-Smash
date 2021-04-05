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
        public event ItemChange onEquippedActionItemChange;

        int currentIndex = 0;
        private void Start()
        {
            ReplenishActionableItems();
        }

        private void OnEnable()
        {
            Tent.OnTentMenuOpen += ReplenishActionableItems;
        }

        private void OnDisable()
        {
            Tent.OnTentMenuOpen -= ReplenishActionableItems;
        }

        public void SetSubWeapon(SubWeaponItem subWeapon)
        {
            this.subWeapon = subWeapon; 
        }

        public SubWeaponItem GetSubWeapon()
        {
            return subWeapon;
        }

        void ReplenishActionableItems()
        {
            foreach (EquipmentSlots slot in slots)
            {
                slot.number = slot.maxNumber;
            }
            onEquippedActionItemChange(currentIndex);
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
            onEquippedActionItemChange(currentIndex);
        }

        public void ItemUsed(EquipmentSlots slot)
        {
            slot.number -= 1;
            onEquippedActionItemChange(currentIndex);
        }

        public EquipmentSlots[] GetActionableItems()
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
                    ReplenishActionableItems();
                    return;
                }
            }

        }

        public object CaptureState()
        {
            Dictionary<string, int> toolsForSerialization = new Dictionary<string, int>();
            foreach(EquipmentSlots slot in slots)
            {
                toolsForSerialization.Add(slot.item.GetID(), slot.number);
            }
            return toolsForSerialization;
        }

        public void RestoreState(object state)
        {

            var equippedItemsForSerialization = (Dictionary<string, int>)state;

            foreach (string name in equippedItemsForSerialization.Keys)
            {
                var item = (ToolItem)Item.GetFromID(name);
                if (item != null)
                {
                    foreach(EquipmentSlots slot in slots)
                    {
                        if(item == slot.item)
                        {
                            print(slot.item.name + "  got  " + equippedItemsForSerialization[name]);
                            slot.maxNumber = equippedItemsForSerialization[name];
                        }
                    }
                }
            }
            //ReplenishActionableItems();
        }
    }
}

