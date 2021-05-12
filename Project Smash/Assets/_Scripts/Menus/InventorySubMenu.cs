using PSmash.Inventories;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Menus
{
    /// <summary>
    /// Creates and Deletes the display of the crafting items in the menus based on selection of the Inventory Tab
    /// </summary>
    public class InventorySubMenu : MonoBehaviour
    {
        [SerializeField] ItemSubMenu item = null;

        private void OnEnable()
        {
            print("Enabling");
            Inventory inventory = Inventory.GetPlayerInventory();
            List<Inventory.CraftingSlot> craftingItemsList = inventory.GetCraftingItemsList();
            foreach (Inventory.CraftingSlot slot in craftingItemsList)
            {
                ItemSubMenu itemClone = Instantiate(item, transform);
                itemClone.Setup(slot);
            }
        }

        private void OnDisable()
        {
            print("Disabling");
            foreach (ItemSubMenu item in GetComponentsInChildren<ItemSubMenu>())
            {
                Destroy(item.gameObject);
            }
        }
    }
}

