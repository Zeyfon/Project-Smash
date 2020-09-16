using System.Collections.Generic;
using UnityEngine;
using PSmash.Resources;

namespace PSmash.Items.Doors
{
    public class Key : MonoBehaviour
    {
        [SerializeField] InteractionList myValue;
        public static Dictionary<InteractionList, int> keys = new Dictionary<InteractionList, int>();

        public delegate void KeyTaken(InteractionList myValue);
        public static event KeyTaken OnKeyTaken;
        AudioSource audioSource;
        bool keyTaken = false;
        public static int quantityRequired;
        private void Awake()
        {
            if (keys.ContainsKey(myValue)) keys[myValue] = keys[myValue] + 1;
            else
            {
                keys.Add(myValue, 1);
            }
            audioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if (keyTaken)
            {
                if (audioSource.isPlaying) return;
                Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                GetComponent<Collider2D>().enabled = false;
                OnKeyTaken(myValue);
                GetComponent<AudioSource>().Play();
                transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                keyTaken = true;
            }
        }
    }

}
