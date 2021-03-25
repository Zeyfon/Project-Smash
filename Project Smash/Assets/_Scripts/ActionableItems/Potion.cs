using PSmash.Attributes;
using UnityEngine;

namespace PSmash.Inventories
{
    public class Potion : UsableItem
    {
        [SerializeField] float health = 50;

        private void Start()
        {
            print("Restoring Health");
            owner.GetComponent<PlayerHealth>().RestoreHealth(health);
        }
    }
}

