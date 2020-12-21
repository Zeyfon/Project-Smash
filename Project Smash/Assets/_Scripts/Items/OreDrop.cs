using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Items
{
    public class OreDrop : MonoBehaviour
    {
        public static event Action OnCurrencyCollected;

        AudioSource audioSource;
        ParticleSystem ps;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            audioSource = GetComponent<AudioSource>();
        }

        private void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            ps.trigger.SetCollider(10, player.GetComponent<Collider2D>());
        }

        void Update()
        {
            if (!ps.IsAlive())
            {
                Destroy(gameObject);
            }
        }
        private void OnParticleTrigger()
        {
            List<ParticleSystem.Particle> inside = new List<ParticleSystem.Particle>();
            int numInside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, inside);
            for (int i = 0; i < numInside; i++)
            {
                ParticleSystem.Particle p = inside[i];
                p.remainingLifetime = 0;
                inside[i] = p;
                if(!audioSource.isPlaying) audioSource.Play();
                OnCurrencyCollected();
            }
            ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, inside);
        }
    }
}

