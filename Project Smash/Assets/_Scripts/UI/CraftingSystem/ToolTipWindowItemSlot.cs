using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using PSmash.Inventories;

namespace PSmash.UI.CraftingSytem
{
    public class ToolTipWindowItemSlot : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI requiredQuantityText = null;
        [SerializeField] TextMeshProUGUI playerQuantityText = null;
        [SerializeField] TextMeshProUGUI middleSeparatorText = null;
        [SerializeField] Image image = null;

        public void UpdateCraftingItem(CraftingItem item, int requiredQuantity, int playerQuantity)
        {
            Color craftingColor;
            //print(material.sprite.name);
            image.sprite = item.GetSprite();
            requiredQuantityText.text = requiredQuantity.ToString();
            playerQuantityText.text = playerQuantity.ToString();
            if (playerQuantity >= requiredQuantity)
                craftingColor = Color.white;
            else
                craftingColor = Color.red;

            requiredQuantityText.color = craftingColor;
            playerQuantityText.color = craftingColor;
            middleSeparatorText.color = craftingColor;
        }
    }
}

