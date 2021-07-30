using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Inventories
{
    public class PickupCollectible : MonoBehaviour
    {
        [SerializeField] Collider2D collectingCollider = null;
        [SerializeField] Transform body;
       // public delegate void DropCollected(ItemSlot slot);
       // public static event DropCollected onDropCollected;

        //List<ItemSlot> slots = new List<ItemSlot>();

        //public void Setup(IEnumerable<DropLibrary.Dropped> drops)
        //{
        //    collectingCollider.enabled = false;
        //    //print("Setting up pickup ");

        //    ///TODO
        //    ///THERE IS A KNOWN BUG THAT SPAWN THE CHEST WITHOUT ANY ITEMS IN IT
        //    ///THE ISSUE IS IN THIS FOREACH LOOP. IT DOES NOT DO IT EVEN THOUGH IN THE ITEMDROPPER THERE IS A SIGNAL THAT THERE IS ACTUALLY 
        //    ///AN ITEM TO DROP
        //   foreach(DropLibrary.Dropped drop in drops)
        //    {
        //        ItemSlot slot = new ItemSlot();
        //        slot.item = drop.item;
        //        slot.number = drop.number;
        //        //print("will drop  " + slot.item);
        //        slots.Add(slot);
        //    }
        //    StartCoroutine(WaitingTime());
        //}

        //IEnumerator WaitingTime()
        //{
        //    yield return new WaitForSeconds(0.25f);
        //    collectingCollider.enabled = true;
        //}

        //public class ItemSlot
        //{
        //    public Item item;
        //    public int number;
        //}

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collectingCollider.enabled = false;
                StartCoroutine(CollectCollectible());
            }
        }

        IEnumerator CollectCollectible()
        {
            body.gameObject.SetActive(false);
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.Play();
            while (audioSource.isPlaying)
            {
                yield return null;
            }
            Destroy(gameObject);

            //foreach(ItemSlot slot in slots)
            //{
            //    onDropCollected(slot);
            //}
        }
    }

}

