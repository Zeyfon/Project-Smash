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
            ActionableItem item = GetComponentInChildren<Inventory>().GetEquippedActionableItem();
            GetComponent<Animator>().SetInteger("UseItem", item.GetAnimatorInt());
        }

        //AnimEvent
        public void SpawnItem()
        {
            ActionableItem item = GetComponentInChildren<Inventory>().GetEquippedActionableItem();
            int quantity = (int)item.GetNumber();
            if (quantity<=0)
            {
                print("Cannot spawn item");
                return;
            }

            print("Spawning Item");
            audioSource.clip = item.GetAudioClip();
            audioSource.Play();
            GameObject itemClone =  Instantiate(item.GetGameObject(), spawner.position, Quaternion.identity);
            itemClone.GetComponent<UsableItem>().SetOwner(GetComponent<Health>());
            if (transform.right.x <0)
                itemClone.transform.eulerAngles = new Vector3(0, 180, 0);
            quantity -= 1;
            print(quantity);
            GetComponentInChildren<Inventory>().ItemUsed(item, quantity);
        }
    }

}
