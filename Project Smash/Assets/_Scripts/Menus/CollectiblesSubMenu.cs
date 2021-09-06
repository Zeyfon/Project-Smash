using PSmash.Inventories;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Menus
{
    /// <summary>
    /// Creates and Deletes the display of the crafting items in the menus based on selection of the Inventory Tab
    /// </summary>
    public class CollectiblesSubMenu : MonoBehaviour
    {
        [SerializeField] CollectibleUI[] collectibles = null;

        private void OnEnable()
        {
            List<CollectibleItem> collectibles = Inventory.GetPlayerInventory().GetCollectibles();
            foreach(CollectibleItem collectibleRecovered in collectibles)
            {
                foreach(CollectibleUI collectibleUI in this.collectibles)
                {
                    if(collectibleUI.GetMyCollectible().GetID() == collectibleRecovered.GetID())
                    {
                        collectibleUI.gameObject.SetActive(true);
                    }
                }
            }

        }

        private void OnDisable()
        {
            //print("Disabling");
            foreach (CollectibleUI item in GetComponentsInChildren<CollectibleUI>())
            {
                item.gameObject.SetActive(false);
            }
        }
    }
}

