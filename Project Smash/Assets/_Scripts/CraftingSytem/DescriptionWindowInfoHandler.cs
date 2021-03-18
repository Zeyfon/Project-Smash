using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Stats;
using PSmash.Items;

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
    public class DescriptionWindowInfoHandler : MonoBehaviour
    {
        [SerializeField] CraftingItemSlotsUI[] craftingItemSlots;
        //MyCraftingMaterials playerMaterials;

        Inventory inventory;
        private void Awake()
        {

            inventory = Inventory.GetPlayerInventory();
            //playerMaterials = GameObject.FindGameObjectWithTag("Player").GetComponent<MyCraftingMaterials>();
        }

        public void SetSkillSlotInfo(Dictionary<CraftingItem, int> requiredCraftingMaterials)
        {
            foreach (CraftingItemSlotsUI craftingItemSlot in craftingItemSlots)
            {
                craftingItemSlot.gameObject.SetActive(true);
            }
            //print("Sending the info to each CraftingMaterialsSlot ");
            int j = 0;
            foreach (CraftingItem craftingItem in requiredCraftingMaterials.Keys)
            {
                int playerQuantity = inventory.GetThisCraftingItemNumber(craftingItem);

                //print("Will send to " + craftingMaterialsSlots[j].gameObject.name + " this " + material.material.ToString() + "  " + requiredCraftingMaterials[material] + "   " + playerQuantity);
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
