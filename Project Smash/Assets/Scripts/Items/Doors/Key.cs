using System.Collections.Generic;
using System.Collections;
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
        bool keyTaken = false;
        public static int quantityRequired;
        private void Awake()
        {
            if (keys.ContainsKey(myValue)) keys[myValue] = keys[myValue] + 1;
            else
            {
                keys.Add(myValue, 1);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                GetComponent<Collider2D>().enabled = false;
                OnKeyTaken(myValue);
                transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                StartCoroutine(AudioPlays());
            }
        }

        IEnumerator AudioPlays()
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.Play();
            while (audioSource.isPlaying)
            {
                yield return new WaitForEndOfFrame();
            }
            Destroy(gameObject);
        }
    }

}
