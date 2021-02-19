using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Items
{
    public class PlayerItems : MonoBehaviour
    {
        [SerializeField] int potions = 2;
        [SerializeField] int daggers = 3;

        public int GetItemQuantity(ItemList item)
        {
            switch (item)
            {
                case ItemList.Potion:
                    return potions;
                case ItemList.Dagger:
                    return daggers;
                default:
                    return 0;
            }
        }

        public void DecreaseThisItemQuantity(ItemList item)
        {
            print("Decreasing " +  item.ToString());
            switch (item)
            {
                case ItemList.Potion:
                    potions--;
                    break;
                case ItemList.Dagger:
                    daggers--;
                    break;
                default:
                    break; ;
            }
        }
    }

}
