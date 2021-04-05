using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Stats;

namespace PSmash.Inventories
{
    [CreateAssetMenu(menuName = "Items/Status Item")]
    public class StatusItem : Item
    {
        [Tooltip("The amount that will be used when the stat gets an upgread by the Crafting System")]
        [SerializeField] float number =  0;
        [Tooltip("The enum ID that will be used to allocate the information within BaseStats")]
        [SerializeField] StatsList myID;

        public float GetNumber()
        {
            return number;
        }

        public StatsList GetMyEnumID()
        {
            return myID;
        }
    }
}
