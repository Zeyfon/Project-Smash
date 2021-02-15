using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        public void SetRequiredMaterials(Dictionary<CraftingMaterialsList, int> requiredMaterials)
        {
            foreach(CraftingMaterialSlot craftingMaterialSlot in craftingMaterialsSlots)
            {
                craftingMaterialSlot.gameObject.SetActive(true);
            }
            print("Sending the info to each CraftingMaterialsSlot " );
            int j = 0;
            foreach(CraftingMaterialsList material in requiredMaterials.Keys)
            {
                craftingMaterialsSlots[j].UpdateMaterialSlot(material, requiredMaterials[material]);
                j++;
            }
            for(int k = j; k < craftingMaterialsSlots.Length; k++)
            {
                craftingMaterialsSlots[k].gameObject.SetActive(false);
            }
        }
    }



}
