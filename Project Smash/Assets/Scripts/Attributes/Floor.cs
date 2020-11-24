using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{
    public class Floor : MonoBehaviour, IDamagable
    {

        [SerializeField] AudioSource audioSource = null;
        public void TakeDamage(Transform attacker, int damage)
        {
            audioSource.pitch = Random.Range(0.7f, 1);
            audioSource.Play();
        }

    }

}
