using System;
using UnityEngine;

namespace PSmash.Items
{
    public class Star : MonoBehaviour
    {
        public static int activeStarsOnSceneQuantity = 0;
        public static event Action OnStarCollected;
        [SerializeField] GameObject starEffectGameObject;

        private void OnEnable()
        {
            activeStarsOnSceneQuantity += 1;
        }

        private void OnDisable()
        {
            activeStarsOnSceneQuantity -= 1;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (OnStarCollected != null) OnStarCollected();
                Instantiate(starEffectGameObject, transform.position, Quaternion.Euler(0, 0, 90));

                Destroy(gameObject);
            }
        }

    }
}

