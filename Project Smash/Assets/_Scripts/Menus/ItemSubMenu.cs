using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PSmash.Inventories;

namespace PSmash.Menus
{
    /// <summary>
    /// Setup in the menu the crafting item for the player to check the quantity in the menus
    /// </summary>
    public class ItemSubMenu : MonoBehaviour
    {
        [SerializeField] Image image = null;
        [SerializeField] TextMeshProUGUI text = null;

        public void Setup(Inventory.CraftingSlot slot)
        {
            image.sprite = slot.item.GetSprite();
            text.text = slot.number.ToString();
        }
    }
}


