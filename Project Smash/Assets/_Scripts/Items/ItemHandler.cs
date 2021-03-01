using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Items
{
    public class ItemHandler : MonoBehaviour, IItemUsed
    {
        [SerializeField] Item potion;
        [SerializeField] Item dagger;

        public delegate void ItemChange(Item item, int quantity);
        public event ItemChange onItemChange;

        int itemIndex;

        // Start is called before the first frame update
        void Start()
        {
            SetCurrentItem(potion);
        }

        public Item GetDagger()
        {
            return dagger;
        }

        public void ChangeItem(bool isMovingRight)
        {
            print("Here " + this + " item changed");
            if (isMovingRight)
            {
                itemIndex++;
                print("Moving right  " + itemIndex);
                if (itemIndex > System.Enum.GetValues(typeof(ItemList)).Length-1)
                {
                    itemIndex = 0;
                }
            }
            else
            {
                itemIndex--;
                print("Moving Left  " + itemIndex);
                if (itemIndex < 0)
                {
                    itemIndex = System.Enum.GetValues(typeof(ItemList)).Length-1;
                }
            }

            //print("ItemHandler item  " + itemIndex);
            switch (itemIndex) 
            {
                case 0:
                    SetCurrentItem(potion);
                    break;
                case 1:
                    SetCurrentItem(dagger);
                    break;
            }

        }

        void SetCurrentItem(Item item)
        {
            int quantity = GetComponentInParent<PlayerItems>().GetItemQuantity(item.item);
            print(item.name + "  " + quantity);
            bool canUseItem;
            if (quantity <= 0)
            {
                canUseItem = false;
            }
            else
            {
                canUseItem = true;
            }

            //Set Current item for Player Use
            GetComponentInParent<UseItem>().SetCurrentEquippedItem(item, canUseItem);

            //Set Current item in UI;
            onItemChange(item,quantity);
        }

        public void ItemUsed(Item item)
        {
            GetComponentInParent<PlayerItems>().DecreaseThisItemQuantity(item.item);
            SetCurrentItem(item);
        }
    }
}

