using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Core;

namespace PSmash.Items
{
    public class Boots : MonoBehaviour
    {
        float timer = 0;
        bool canTakeTheBoots = false;

        // Update is called once per frame
        void Update()
        {
            if (timer < 0.5f)
            {
                timer += Time.deltaTime;
            }
            else
            {
                Debug.Log("Can Take Boots Now");
                canTakeTheBoots = true;
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (!canTakeTheBoots) return;
            if (collision.collider.CompareTag("Player"))
            {
                GameObject.FindObjectOfType<EventManager>().PlayerGotTheBoots();
                Destroy(gameObject);
            }

        }
    }
}

