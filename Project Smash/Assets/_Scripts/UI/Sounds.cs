using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;

namespace PSmash.UI
{
    public class Sounds : MonoBehaviour
    {
        [SerializeField] AudioClip itemChangeSound = null;
        AudioSource audioSource;
        // Start is called before the first frame update
        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        private void OnEnable()
        {
            EventManager.ItemChanged += ItemChanged;
        }

        private void OnDisable()
        {
            EventManager.ItemChanged -= ItemChanged;
        }

        public void ItemChanged(int item)
        {
            int item2 = item;
            audioSource.PlayOneShot(itemChangeSound);
        }
    }
}

