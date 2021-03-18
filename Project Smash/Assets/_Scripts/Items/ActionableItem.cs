using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Items
{
    [CreateAssetMenu(menuName = "Items/Action Item")]
    public class ActionableItem : Item
    {
        [SerializeField] GameObject gameObject = null;
        [SerializeField] int animatorIntValue = 0;
        [SerializeField] bool consumable = false;
        [SerializeField] float maxNumber = 0;
        [SerializeField] AudioClip useItemClip = null;

        float number;

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

        public float GetMaxNumber()
        {
            return maxNumber;
        }

        public void SetNumber(float number)
        {
            this.number = number;
        }

        public void IncreaseQuantity()
        {
            maxNumber++;
            number = maxNumber;
        }

        public float GetNumber()
        {
            return number;
        }
    }
}
