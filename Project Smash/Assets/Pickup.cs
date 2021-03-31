using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Inventories
{
    public class Pickup : MonoBehaviour
    {
        [SerializeField] Collider2D collectingCollider = null;
        public delegate void DropCollected(ItemSlot slot);
        public static event DropCollected onDropCollected;

        List<ItemSlot> slots = new List<ItemSlot>();

        public void Setup(IEnumerable<DropLibrary.Dropped> drops)
        {
            collectingCollider.enabled = false;
           foreach(var drop in drops)
            {
                ItemSlot slot = new ItemSlot();
                slot.item = drop.item;
                slot.number = drop.number;
                slots.Add(slot);
            }
            StartCoroutine(WaitingTime());
        }

        IEnumerator WaitingTime()
        {
            yield return new WaitForSeconds(0.25f);
            collectingCollider.enabled = true;
        }

        public class ItemSlot
        {
            public Item item;
            public int number;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                CollectDrops();
                Destroy(gameObject);
            }
        }

        void CollectDrops()
        {
            foreach(ItemSlot slot in slots)
            {
                onDropCollected(slot);
            }
        }
    }

}

