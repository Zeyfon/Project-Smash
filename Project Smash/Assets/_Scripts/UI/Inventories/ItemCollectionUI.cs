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
            Pickup.onDropCollected += ShowItemCollected;
        }

        private void OnDisable()
        {
            Pickup.onDropCollected -= ShowItemCollected;
        }

        private void ShowItemCollected(Pickup.ItemSlot pickupSlot)
        {
            ItemCollecteSlotUI slotClone = Instantiate(slot, transform);
            slotClone.Setup(pickupSlot);
        }
    }

}
