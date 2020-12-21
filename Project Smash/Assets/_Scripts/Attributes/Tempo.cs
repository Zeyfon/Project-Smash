using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{
    public class Tempo : MonoBehaviour
    {
        AudioSource audioSource;

        void Start()
        {
            audioSource = GetComponent<AudioSource>();
        }

        void ClockTickSound()
        {
            audioSource.Play();
        }
    }

}
