using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PSmash.Items;
using UnityEngine.UI;

namespace PSmash.UI
{
    public class UIItem : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text = null;
        [SerializeField] Image image = null;

        public void UpdateItemInfo(InventoryItems.Items item)
        {
            image.sprite = item.item.sprite;
            if (text == null)
                return;
            text.enabled = true;
            text.text = item.quantity.ToString();
        }
    }

}
