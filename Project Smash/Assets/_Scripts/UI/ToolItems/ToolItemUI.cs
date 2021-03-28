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
            //print(playerInventory);
        }

        private void OnEnable()
        {
            playerEquipment.onEquippedActionItemChange += EquippedActionItemChange;
        }

        private void OnDisable()
        {
            playerEquipment.onEquippedActionItemChange -= EquippedActionItemChange;
        }

        private void EquippedActionItemChange(int index)
        {
            //print("Update Action Items UI ");
            //List<ActionableItem> items = playerInventory.GetActionableItems();
            Equipment.EquipmentSlots[] slots = playerEquipment.GetActionableItems();

            int previousIndex = GetPreviousItem(slots, index);

            leftItem.UpdateItemInfo(slots[previousIndex]);
            centerItem.UpdateItemInfo(slots[index]);
        }

        int GetPreviousItem(Equipment.EquipmentSlots[] slots, int index)
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

