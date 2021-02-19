using PSmash.Attributes;
using UnityEngine;

namespace PSmash.Items
{
    public class Potion : MonoBehaviour
    {
        [SerializeField] float health = 50;

        private void Start()
        {
            print("Restoring Health");
            FindObjectOfType<PlayerHealth>().RestoreHealth(health);
        }
    }
}

