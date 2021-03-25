using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Inventories
{
    [CreateAssetMenu(menuName = "Items/Action Item")]
    public class ActionableItem : Item
    {
        [SerializeField] GameObject gameObject = null;
        [SerializeField] int animatorIntValue = 0;
        [SerializeField] bool consumable = false;
        [SerializeField] int maxNumber = 0;
        [SerializeField] AudioClip useItemClip = null;

        int number;

        public int GetAnimatorInt()
        {
            return animatorIntValue;
        }
        public GameObject GetGameObject()
        {
            return gameObject;
        }
        public AudioClip GetAudioClip()
        {
            return useItemClip;
        }
        public bool IsConsumable()
        {
            return consumable;
        }

        public int GetMaxNumber()
        {
            return maxNumber;
        }

        public void SetNumber(int number)
        {
            this.number = number;
        }

        public void IncreaseQuantity()
        {
            maxNumber++;
            number = maxNumber;
        }

        public int GetNumber()
        {
            return number;
        }
    }
}
