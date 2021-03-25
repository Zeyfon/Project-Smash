using PSmash.Inventories;
using System.Collections.Generic;
using UnityEngine;
using PSmash.UI.CraftingSytem;

namespace PSmash.UI
{
    /// <summary>
    /// The idea is that this script will be in charge of updating the required materials for that specificil SkillSlot
    /// When the player change the currentSelectedGameObject from the EventSystem that will trigger a event for the DescriptionWindow to retreive the information
    /// from that particulas SkillSlot about what materials are required for it. 
    /// Then once gotten the Dictionary of the Crafting material and its required number it will pass that information to the ListManager and it will only
    /// put that information into each slot
    /// For the remaning materials slots that are not required they will be disabled for the player not to see those.
    /// The Updating will take place during the FadeOut and Fade In of the Description Window for the player not see the changes done
    /// </summary>
    public class ToolTipCraftingItemsHandler : MonoBehaviour
    {
        [SerializeField] CraftingItemSlotsUI[] craftingItemSlots;

        Inventory inventory;
        private void Awake()
        {
            inventory = Inventory.GetPlayerInventory();
        }

        public void SetSkillSlotInfo(Dictionary<CraftingItem, int> requiredCraftingMaterials)
        {
            foreach (CraftingItemSlotsUI craftingItemSlot in craftingItemSlots)
            {
                craftingItemSlot.gameObject.SetActive(true);
            }
            int j = 0;
            foreach (CraftingItem craftingItem in requiredCraftingMaterials.Keys)
            {
                int playerQuantity = inventory.GetThisCraftingItemNumber(craftingItem);
                craftingItemSlots[j].UpdateCraftingItem(craftingItem, requiredCraftingMaterials[craftingItem], playerQuantity);
                j++;
            }
            for (int k = j; k < craftingItemSlots.Length; k++)
            {
                craftingItemSlots[k].gameObject.SetActive(false);
            }
        }
    }



}
