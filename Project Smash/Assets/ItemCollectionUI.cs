using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Inventories;
using System;

namespace PSmash.UI.Inventories
{
    public class ItemCollectionUI : MonoBehaviour
    {
        [SerializeField] ItemCollecteSlotUI slot = null;

        int number = 0;
        private void OnEnable()
        {
            Drop.onDropCollected += ShowItemCollected;
        }

        private void OnDisable()
        {
            Drop.onDropCollected -= ShowItemCollected;
        }

        private void ShowItemCollected(CraftingItem item)
        {
            GameObject slotClone = Instantiate(slot.gameObject, transform);
            slotClone.GetComponent<ItemCollecteSlotUI>().Setup(item, number);
            number++;
        }
    }

}
