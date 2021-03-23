using PSmash.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PSmash.Items.Doors
{
    public class Key : MonoBehaviour
    {
        [SerializeField] InteractionList myID;
        public static Dictionary<InteractionList, int> keys = new Dictionary<InteractionList, int>();

        public delegate void KeyTaken(InteractionList myValue);
        public static event KeyTaken OnKeyTaken;

        //During Awake this script will check how many keys for this ID actually exist in the Scene
        //by adding one for each ID to the dictionary
        private void Awake()
        {
            if (keys.ContainsKey(myID)) keys[myID] = keys[myID] + 1;
            else
            {
                keys.Add(myID, 1);
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

        // This is implemented for when the player dies and the dictionary must be cleared for the next try.
        private void OnSceneUnload(Scene arg0)
        {
            if (keys.ContainsKey(myID))
            {
                print("Keys dictionary cleared");
                keys.Clear();
            }
        }

        //This Coroutine will play the audio when the key is taken by the player
        //and will wait till it ends to destroy the object
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

        //Looking for player, when in contact will disable the sprite
        //and start a AudioPlay Coroutine
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                GetComponent<Collider2D>().enabled = false;
                OnKeyTaken(myID);
                transform.GetChild(0).GetComponent<SpriteRenderer>().enabled = false;
                StartCoroutine(AudioPlays());
            }
        }
    }

}
