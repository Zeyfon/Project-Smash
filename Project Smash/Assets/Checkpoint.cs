﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Saving;
using System;

namespace PSmash.Checkpoints
{
    public class Checkpoint : MonoBehaviour
    {
        [SerializeField] AudioSource audioSource = null;
        // Start is called before the first frame update

        public static event Action onCheckpointPerformed;
        void Start()
        {

        }

        private void OnEnable()
        {
            Tent.OnTentMenuOpen += PerformCheckpoint;
        }


        void PerformCheckpoint()
        {
            onCheckpointPerformed();
            //DoCheckpointSound();oint
            //ResetInteractables();
            //ResetDestructibleObjects();
        }


        void DoCheckpointSound()
        {
            audioSource.Play();
        }

        void ResetInteractables()
        {

        }
    }

}