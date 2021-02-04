using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{
    [RequireComponent(typeof(AudioSource))]
    public class Ground : MonoBehaviour, IDamagable
    {

        [SerializeField] AudioClip damageObjectSound = null;
        AudioSource audioSource = null;
        void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            audioSource.clip = damageObjectSound;
            audioSource.playOnAwake = false;
        }
        public void TakeDamage(Transform attacker, WeaponList weapon, float damage)
        {
            audioSource.pitch = Random.Range(0.7f, 1);
            audioSource.Play();
        }

    }

}
