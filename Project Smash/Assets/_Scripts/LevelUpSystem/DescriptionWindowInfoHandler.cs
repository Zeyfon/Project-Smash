using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Stats;

namespace PSmash.LevelUpSystem
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
        [SerializeField] CraftingMaterialSlot[] craftingMaterialsSlots;
        MyCraftingMaterials playerMaterials;

        private void Awake()
        {
            playerMaterials = GameObject.FindGameObjectWithTag("Player").GetComponent<MyCraftingMaterials>();
        }

        public void SetCurrentSkillSlotMaterials(Dictionary<CraftingMaterial, int> requiredCraftingMaterials)
        {
            foreach (CraftingMaterialSlot craftingMaterialSlot in craftingMaterialsSlots)
            {
                craftingMaterialSlot.gameObject.SetActive(true);
            }
            //print("Sending the info to each CraftingMaterialsSlot ");
            int j = 0;
            foreach (CraftingMaterial material in requiredCraftingMaterials.Keys)
            {
                int playerQuantity = playerMaterials.GetPlayerQuantityForThisMaterial(material);

                //print("Will send to " + craftingMaterialsSlots[j].gameObject.name + " this " + material.material.ToString() + "  " + requiredCraftingMaterials[material] + "   " + playerQuantity);
                craftingMaterialsSlots[j].UpdateCraftingMaterial(material, requiredCraftingMaterials[material], playerQuantity);
                j++;
            }
            for (int k = j; k < craftingMaterialsSlots.Length; k++)
            {
                craftingMaterialsSlots[k].gameObject.SetActive(false);
            }
        }
    }



}
