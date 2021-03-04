using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PSmash.Combat;
using PSmash.Items;
using System;

namespace PSmash.UI
{
    public class UIItemHandler : MonoBehaviour
    {
        [SerializeField] UIItem centerItem = null;
        [SerializeField] UIItem leftItem = null;

        private void OnEnable()
        {
            FindObjectOfType<ItemHandler>().onItemChange += ItemChange;
        }

        private void OnDisable()
        {
            FindObjectOfType<ItemHandler>().onItemChange -= ItemChange;
        }

        private void ItemChange(InventoryItems.Items[] items)
        {
            leftItem.UpdateItemInfo(items[0]);
            centerItem.UpdateItemInfo(items[1]);
        }
    }
}

