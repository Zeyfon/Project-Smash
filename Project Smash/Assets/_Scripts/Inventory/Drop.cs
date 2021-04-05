using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Inventories
{
    public class Drop : MonoBehaviour
    {
        [SerializeField] CraftingItem item = null;
        [SerializeField] Collider2D trigger = null;

        public delegate void DropCollected(CraftingItem material);
        public static event DropCollected onDropCollected;

        private void Awake()
        {
            GetComponentInChildren<SpriteRenderer>().sprite = item.GetSprite();
            //StartCoroutine(Setup());
        }

        IEnumerator Setup()
        {
            trigger.enabled = false;
            yield return new WaitForSeconds(1);
            trigger.enabled = true;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                onDropCollected(item);
                Destroy(gameObject);
            }
        }
    }
}

