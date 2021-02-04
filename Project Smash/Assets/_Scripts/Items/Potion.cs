using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Items
{
    public class Potion : MonoBehaviour
    {
        [SerializeField] float health = 50;
        [SerializeField] int usages = 3;

        public float GetHealthValue()
        {
            return health;
        }

        public int GetPotionsUsages()
        {
            return usages;
        }

        public void SetPotionsUsages(int usages)
        {
            this.usages = usages;
        }
    }
}

