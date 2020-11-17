using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEngine;
using PSmash.Combat;
using PSmash.Resources;
using UnityEngine.Events;

namespace PSmash.Items
{
    public class Blockage : MonoBehaviour, IDamagable
    {
        [SerializeField] AudioClip damagedSound = null;
        [SerializeField] AudioClip destroyedSound = null;
        [SerializeField] int hitsToDestroyWall = 4;
        [SerializeField] ParticleSystem particles;
        [SerializeField] float shakeAmount = 0.2f;
        [SerializeField] UnityEvent OnBlockageDestroyed;
        AudioSource audioSource;
        Coroutine coroutine;
        int hits = 0;

        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void TakeDamage(Transform attacker, int damage)
        {
            if (hits >= hitsToDestroyWall)
            {
                GetComponent<Collider2D>().enabled = false;
                WallDestroyed();
            }
            else WallDamaged();
            hits++;
        }

        void WallDestroyed()
        {
            GetComponent<Collider2D>().enabled = false;
            print("Wall Destroyed");
            audioSource.PlayOneShot(destroyedSound);
            OnBlockageDestroyed.Invoke();
            particles.Stop();
            StopCoroutine(coroutine);
            GetComponent<Animation>().Play();
        }

        void WallDamaged()
        {
            print("Particles Playing");
            particles.Play();
            audioSource.PlayOneShot(damagedSound);
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(Shake(transform.position, shakeAmount));
        }

        IEnumerator Shake(Vector3 initialPosition, float shakeAmount)
        {
            while (shakeAmount > 0)
            {
                shakeAmount -= Time.deltaTime;
                if (shakeAmount < 0) shakeAmount = 0;
                transform.position = initialPosition + (Vector3)UnityEngine.Random.insideUnitCircle * shakeAmount;
                yield return new WaitForEndOfFrame();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                print("Triggered");
                transform.parent.GetComponent<CoveredArea>().ShowHiddenArea();
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }

}
