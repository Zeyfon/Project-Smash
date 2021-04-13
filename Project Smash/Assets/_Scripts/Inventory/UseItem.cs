using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;

namespace PSmash.Inventories
{
    public class UseItem : MonoBehaviour
    {
        [SerializeField] Transform spawner = null;
        [SerializeField] AudioSource audioSource = null;

        //Called by the UseItemState FSM in Playmaker
        public void UseEquippedItem()
        {
            Equipment.EquipmentSlots slot = GetComponentInChildren<Equipment>().GetEquipmentSlot();
            GetComponent<Animator>().SetInteger("UseItem", slot.item.GetAnimatorInt());
            print("Using Equipped Tool");
        }

        //AnimEvent
        public void SpawnItem()
        {
            Equipment.EquipmentSlots slot = GetComponentInChildren<Equipment>().GetEquipmentSlot();
            if (slot.number <= 0)
            {
                print("Cannot spawn item");
                return;
            }

            print("Spawning Item");
            audioSource.clip = slot.item.GetAudioClip();
            audioSource.Play();
            GameObject itemClone =  Instantiate(slot.item.GetGameObject(), spawner.position, Quaternion.identity);
            itemClone.GetComponent<UsableItem>().SetOwner(GetComponent<Health>());
            if (transform.right.x <0)
                itemClone.transform.eulerAngles = new Vector3(0, 180, 0);
            GetComponentInChildren<Equipment>().ItemUsed(slot);
        }
    }

}
