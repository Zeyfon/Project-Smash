using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;
using Spine.Unity;

namespace PSmash.Inventories
{
    public class UseItem : MonoBehaviour
    {
        [SerializeField] Transform spawner = null;
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] Material defaultMaterial = null;
        [SerializeField] Material greenMaterial = null;

        //Called by the UseItemState FSM in Playmaker
        public void UseEquippedItem()
        {
            Equipment.ToolSlot slot = GetComponentInChildren<Equipment>().GetCurrentEquipmentSlot();
            GetComponent<Animator>().SetInteger("UseItem", slot.item.GetAnimatorInt());
            print("Using Equipped Tool");
        }

        //AnimEvent
        public void SpawnItem()
        {
            Equipment.ToolSlot slot = GetComponentInChildren<Equipment>().GetCurrentEquipmentSlot();
            if (slot.number <= 0)
            {
                print("Cannot spawn item");
                return;
            }

            if(slot.item.GetDisplayName() == "Potions")
            {
                TintPlayerGreen();
            }
            //print("Spawning Item");
            audioSource.clip = slot.item.GetAudioClip();
            audioSource.Play();
            GameObject itemClone =  Instantiate(slot.item.GetGameObject(), spawner.position, Quaternion.identity);
            itemClone.GetComponent<UsableItem>().SetOwner(GetComponent<Health>());
            if (transform.right.x <0)
                itemClone.transform.eulerAngles = new Vector3(0, 180, 0);
            GetComponentInChildren<Equipment>().ItemUsed(slot);
        }

        void TintPlayerGreen()
        {
            //print("Tinting Player Green");
            SkeletonRenderer skeletonRenderer = GetComponent<SkeletonRenderer>();
            skeletonRenderer.CustomMaterialOverride.Add(defaultMaterial, greenMaterial);
        }

        //AnimEvent
        void BackToNormalTint()
        {
            SkeletonRenderer skeletonRenderer = GetComponent<SkeletonRenderer>();
            skeletonRenderer.CustomMaterialOverride.Remove(defaultMaterial);
        }
    }

}
