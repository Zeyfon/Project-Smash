using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Stats;

namespace PSmash.Inventories
{
    [CreateAssetMenu(menuName = "Items/Status Item")]
    public class StatusItem : Item
    {
        [SerializeField] float number =  0;
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
