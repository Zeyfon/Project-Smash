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

        Item currentEquippedItem;
        bool canUseCurrentItem = true;

        public void SetCurrentEquippedItem(Item item, bool canUseCurrentItem)
        {
            currentEquippedItem = item;
            this.canUseCurrentItem = canUseCurrentItem;
            //print("Current item equipped is  " + currentEquippedItem);
        }

        //Called by the UseItemState FSM in Playmaker
        public void UseEquippedItem()
        {
            GetComponent<Animator>().SetInteger("UseItem", currentEquippedItem.animatorIntValue);
        }

        //AnimEvent
        public void SpawnItem()
        {
            if (!canUseCurrentItem)
            {
                print("Cannot spawn item");
                return;
            }
            print("Spawning Item");
            audioSource.clip = currentEquippedItem.useItemAudioClip;
            audioSource.Play();
            GameObject itemClone =  Instantiate(currentEquippedItem.gameObject, spawner.position, Quaternion.identity);
            itemClone.GetComponent<UsableItem>().SetOwner(GetComponent<Health>());
            if (transform.right.x <0)
                itemClone.transform.eulerAngles = new Vector3(0, 180, 0);
            GetComponentInChildren<IItemUsed>().ItemUsed(currentEquippedItem);
        }
    }

}
