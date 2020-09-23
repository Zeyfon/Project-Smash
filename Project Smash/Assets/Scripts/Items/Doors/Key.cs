using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using PSmash.Resources;
using UnityEngine.SceneManagement;
using System;

namespace PSmash.Items.Doors
{
    public class Key : MonoBehaviour
    {

        [SerializeField] InteractionList myValue;
        public static Dictionary<InteractionList, int> keys = new Dictionary<InteractionList, int>();

        public delegate void KeyTaken(InteractionList myValue);
        public static event KeyTaken OnKeyTaken;

        private void Awake()
        {
            if (keys.ContainsKey(myValue)) keys[myValue] = keys[myValue] + 1;
            else
            {
                keys.Add(myValue, 1);
            }
        }

        private void OnEnable()
        {
            SceneManager.sceneUnloaded += OnSceneUnload;
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded += OnSceneUnload;
        }

        private void OnSceneUnload(Scene arg0)
        {
            if (keys.ContainsKey(myValue))
            {
                print("Keys dictionary cleared");
                keys.Clear();
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
