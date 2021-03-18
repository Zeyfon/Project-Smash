using PSmash.Items;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.UI
{
    public class ActionItemUI : MonoBehaviour
    {
        [SerializeField] ActionItemSlotUI centerItem = null;
        [SerializeField] ActionItemSlotUI leftItem = null;

        Inventory playerInventory;
        private void Awake()
        {
            playerInventory = Inventory.GetPlayerInventory();
            //print(playerInventory);
        }

        private void OnEnable()
        {
            playerInventory.onEquippedActionItemChange += EquippedActionItemChange;
        }

        private void OnDisable()
        {
            playerInventory.onEquippedActionItemChange -= EquippedActionItemChange;
        }

        private void EquippedActionItemChange(int index)
        {
            print("Update Action Items UI ");
            List<ActionableItem> items = playerInventory.GetActionableItems();

            int previousIndex = GetPreviousItem(items, index);

            leftItem.UpdateItemInfo(items[previousIndex]);
            centerItem.UpdateItemInfo(items[index]);
        }

        int GetPreviousItem(List<ActionableItem> items, int index)
        {
            int newIndex;
            if (index == 0)
            {
                newIndex = items.Count - 1;
            }
            else
            {
                newIndex = index - 1;
            }
            return newIndex;
        }
    }
}

