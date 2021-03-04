using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;

namespace PSmash.Items
{
    public class UseItem : MonoBehaviour
    {
        [SerializeField] Transform spawner = null;
        [SerializeField] AudioSource audioSource = null;

        //Called by the UseItemState FSM in Playmaker
        public void UseEquippedItem()
        {
            InventoryItems.Items item = GetComponentInChildren<ItemHandler>().GetEquippedItem();
            GetComponent<Animator>().SetInteger("UseItem", item.item.animatorIntValue);
        }

        //AnimEvent
        public void SpawnItem()
        {
            InventoryItems.Items item = GetComponentInChildren<ItemHandler>().GetEquippedItem();
            int quantity = item.quantity;
            if (quantity<=0)
            {
                print("Cannot spawn item");
                return;
            }

            print("Spawning Item");
            audioSource.clip = item.item.useItemAudioClip;
            audioSource.Play();
            GameObject itemClone =  Instantiate(item.item.gameObject, spawner.position, Quaternion.identity);
            itemClone.GetComponent<UsableItem>().SetOwner(GetComponent<Health>());
            if (transform.right.x <0)
                itemClone.transform.eulerAngles = new Vector3(0, 180, 0);
            quantity -= 1;
            print(quantity);
            GetComponentInChildren<ItemHandler>().ItemUsed(item, quantity);
        }
    }

}
