using PSmash.Inventories;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.UI
{
    public class ToolItemUI : MonoBehaviour
    {
        [SerializeField] ToolItemSlotUI centerItem = null;
        [SerializeField] ToolItemSlotUI leftItem = null;

        Equipment playerEquipment;
        private void Awake()
        {
            playerEquipment = Inventory.GetPlayerInventory().GetComponentInParent<Equipment>();
        }

        private void OnEnable()
        {
            playerEquipment.onToolEquippedUpdate += EquipmentUIToolUpdate;
        }

        private void OnDisable()
        {
            playerEquipment.onToolEquippedUpdate -= EquipmentUIToolUpdate;
        }

        private void EquipmentUIToolUpdate(int index)
        {
            Equipment.EquipmentSlots[] slots = playerEquipment.GetTools();

            int previousIndex = GetPreviousIndex(slots, index);
            leftItem.UpdateItemInfo(slots[previousIndex]);

            centerItem.UpdateItemInfo(slots[index]);
        }

        int GetPreviousIndex(Equipment.EquipmentSlots[] slots, int index)
        {
            int newIndex;
            if (index == 0)
            {
                newIndex = slots.Length - 1;
            }
            else
            {
                newIndex = index - 1;
            }
            return newIndex;
        }
    }
}

