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
        //[SerializeField] Text text = null;

        Item previousItem;
        // Start is called before the first frame update
        void Awake()
        {
            previousItem = FindObjectOfType<ItemHandler>().GetDagger();
            UpdateCurrentItem(previousItem);
        }

        private void OnEnable()
        {
            //FindObjectOfType<PlayerFighter>().onItemThrown += ItemUsed;
            FindObjectOfType<ItemHandler>().onItemChange += ItemChange;
        }

        private void OnDisable()
        {
            //FindObjectOfType<PlayerFighter>().onItemThrown -= ItemUsed;
            //FindObjectOfType<ItemHandler>().onItemChange -= ItemChange;
        }

        private void ItemChange(Item currentItem, int quantity)
        {
            UpdateCurrentItem(currentItem, quantity);
            if (previousItem != null && previousItem != currentItem)
            {
                print(previousItem.name + "  " + currentItem.name);
                UpdateCurrentItem(previousItem);
            }
            previousItem = currentItem;
        }

        private void UpdateCurrentItem(Item item, int quantity)
        {
            centerItem.UpdateItemInfo(item, quantity);
        }
        
        void UpdateCurrentItem(Item item)
        {
            leftItem.UpdateItemInfo(item);
        }

        private void UpdatePreviousItem(Item leftItem)
        {
            throw new NotImplementedException();
        }



        //void ItemUsed(int currentQuantity)
        //{
        //    text.text = currentQuantity.ToString();
        //}
    }
}

