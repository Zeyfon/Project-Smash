using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Items
{
    [CreateAssetMenu(fileName = "New Item", menuName = "Items")]
    public class Item : ScriptableObject
    {
        public GameObject gameObject;
        public int animatorIntValue;
        public Sprite sprite;
        public ItemList item;
        public AudioClip useItemAudioClip;
        public string description;

    }
}
