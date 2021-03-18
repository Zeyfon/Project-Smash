using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.LevelUpSystem;

namespace PSmash.Items
{
    public class EnemyDrop : MonoBehaviour
    {
        //[SerializeField] CraftingMaterialsList myMaterial;
        [SerializeField] CraftingItem myMaterial2;

        public delegate void DropCollected(CraftingMaterialsList material);
        public static event DropCollected onDropCollected;

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
            ps.textureSheetAnimation.SetSprite(0, myMaterial2.GetSprite());
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
            //print(inside.Count.ToString());
            int numInside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, inside);
            //print(myMaterial.ToString() + "  checking for  " + numInside);
            for (int i = 0; i < numInside; i++)
            {
                ParticleSystem.Particle p = inside[i];
                p.remainingLifetime = 0;
                inside[i] = p;
                if(!audioSource.isPlaying) audioSource.Play();
                //TODO
                //onDropCollected(myMaterial2.material);
            }
            ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, inside);
        }
    }
}

