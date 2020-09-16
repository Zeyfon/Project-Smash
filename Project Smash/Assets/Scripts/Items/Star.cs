using System;
using UnityEngine;

namespace PSmash.Items
{
    public class Star : MonoBehaviour
    {
        public static int starsQuantity = 0;
        public static event Action OnStarCollected;
        [SerializeField] GameObject starEffectGameObject;

        //This Awake Functions works to know how many stars are instantiated inside the scene. 
        //DO NOT DELETE

        private void Awake()
        {
            starsQuantity += 1;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                OnStarCollected();
                Instantiate(starEffectGameObject, transform.position, Quaternion.Euler(0, 0, 90));

                Destroy(gameObject);
            }
        }

    }
}

