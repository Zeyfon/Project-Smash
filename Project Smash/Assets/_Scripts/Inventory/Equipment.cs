using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Saving;
using System;

namespace PSmash.Inventories
{
    public class Equipment : MonoBehaviour
    {
        [SerializeField] EquipmentSlots[] slots;

        public delegate void ItemChange(int index);
        public event ItemChange onEquippedActionItemChange;

        int currentIndex = 0;
        private void Start()
        {
            foreach(EquipmentSlots slot in slots)
            {
                slot.maxNumber = slot.number;
            }
            ReplenishActionableItems();
        }

        private void OnEnable()
        {
            Tent.OnTentMenuOpen += ReplenishActionableItems;
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

        public void ChangeItem(bool isMovingRight)
        {
            print(currentIndex);
            if (isMovingRight)
            {
                currentIndex++;
                print("Moving right" + currentIndex);
                if (currentIndex > slots.Length - 1)
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
            public ActionableItem item;
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
    }
}

