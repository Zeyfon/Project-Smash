using PSmash.Attributes;
using PSmash.Combat.Weapons;
using PSmash.Combat;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace PSmash.Items
{
    public class Blockage : MonoBehaviour, IDamagable
    {
        [SerializeField] float health = 40;
        [SerializeField] AudioClip damagedSound = null;
        [SerializeField] AudioClip destroyedSound = null;
        [SerializeField] ParticleSystem particles;
        [SerializeField] float shakeAmount = 0.2f;
        [SerializeField] UnityEvent OnBlockageDestroyed;

        AudioSource audioSource;
        Coroutine coroutine;

        
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage, float attackForce)
        {
            if (health <= 0)
            {
                WallDestroyed();
            }
            else WallDamaged();
            health -= damage;
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
            GetComponentInChildren<SpriteRenderer>().enabled = false;
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

        //This is issued for the areas that will be covered to not let the player see them at first
        //but once passed an specific trigger the area will be shown to the player
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                print(transform.parent.gameObject.name );
                transform.parent.GetComponent<CoveredArea>().ShowHiddenArea();
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }

}
