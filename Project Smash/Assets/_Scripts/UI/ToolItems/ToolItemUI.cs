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
        void Awake()
        {
            playerEquipment = Inventory.GetPlayerInventory().GetComponentInParent<Equipment>();
        }

        void OnEnable()
        {
            playerEquipment.onCurrentToolEquippedChange += EquipmentUIToolUpdate;
        }

        void OnDisable()
        {
            playerEquipment.onCurrentToolEquippedChange -= EquipmentUIToolUpdate;
        }

        void EquipmentUIToolUpdate(int index)
        {
            Equipment.ToolSlot[] slots = playerEquipment.GetTools();

            int previousIndex = GetPreviousIndex(slots, index);
            leftItem.UpdateItemInfo(slots[previousIndex]);

            centerItem.UpdateItemInfo(slots[index]);
        }

        int GetPreviousIndex(Equipment.ToolSlot[] slots, int index)
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

