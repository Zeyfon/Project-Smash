using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Inventories
{
    public class Item : ScriptableObject, ISerializationCallbackReceiver
    {
        //CONFIG
        [Header("CONFIG")]
        [SerializeField] Sprite sprite;
        [SerializeField][TextArea] string description;
        [SerializeField] string itemID = " ";
        
        // STATE
        static Dictionary<string, Item> itemLookupCache;


        //PUBLIC

        public Sprite GetSprite()
        {
            return sprite;
        }
        /// <summary>
        /// Get the inventory item instance from its UUID.
        /// </summary>
        /// <param name="itemID">
        /// String UUID that persists between game instances.
        /// </param>
        /// <returns>
        /// Inventory item instance corresponding to the ID.
        /// </returns>
        public static Item GetFromID(string itemID)
        {
            if (itemLookupCache == null)
            {
                itemLookupCache = new Dictionary<string, Item>();
                var itemList = Resources.LoadAll<Item>("");
                foreach (var item in itemList)
                {
                    if (itemLookupCache.ContainsKey(item.itemID))
                    {
                        Debug.LogError(string.Format("Looks like there's a duplicate GameDevTV.UI.InventorySystem ID for objects: {0} and {1}", itemLookupCache[item.itemID], item));
                        continue;
                    }

                    itemLookupCache[item.itemID] = item;
                }
            }

            if (itemID == null || !itemLookupCache.ContainsKey(itemID)) return null;
            return itemLookupCache[itemID];
        }


        // PRIVATE

        public void OnBeforeSerialize()
        {
            if (string.IsNullOrWhiteSpace(itemID))
            {
                // Generate and save a new UUID if this is blank.
                itemID = System.Guid.NewGuid().ToString();
            }
        }

        public void OnAfterDeserialize()
        {
            // Require by the ISerializationCallbackReceiver but we don't need
            // to do anything with it.
        }
    }
}
