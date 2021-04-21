using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;
using PSmash.Combat.Weapons;
using PSmash.Combat;
using PSmash.Inventories;
using GameDevTV.Saving;

namespace PSmash.Items
{
    public class DestructibleObject : MonoBehaviour, IDamagable, ISaveable
    {
        [Header("General")]
        [SerializeField] int hitsToDestroy = 3;

        [Header("Damaged")]
        [SerializeField] AudioSource damageAudioSource = null;

        [Header("Destroyes")]
        [SerializeField] AudioSource destroyedAudioSource = null;
        [SerializeField] GameObject ps = null;
        [SerializeField] SpriteRenderer spriteRender = null;
        [SerializeField] Sprite destroyedSprite = null;

        [SerializeField] ObjectType objectType;


        public static List<string> destroyedObjects = new List<string>();

        public enum ObjectType
        {
            barrel,
            crate,
            rock
        }

        int counter = 0;

        public void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage)
        {
            counter++;
            if (counter > hitsToDestroy)
                return;
            if (counter == hitsToDestroy)
                Destroy();
            else
                Damage();
        }

        void Damage()
        {
            //print(gameObject.name + " received damage  " + " Counter is  " + counter);
            damageAudioSource.Play();
        }

        void Destroy()
        {
            //print(gameObject.name + "  is destroyed");
            destroyedAudioSource.Play();
            GameObject psClone = Instantiate(ps, transform.position, Quaternion.identity, transform);
            StartCoroutine(DestroyParticles(psClone));
            DestroyedState();
            GetComponent<ItemDropper>().RandomDrop();
            string uniqueIdentifier = GetComponent<SaveableEntity>().GetUniqueIdentifier();
            destroyedObjects.Add(uniqueIdentifier);
        }

        private void DestroyedState()
        {
            GetComponent<Collider2D>().enabled = false;

            ///ENABLE A DESTROYED SPRITE
            if (destroyedSprite == null)
            {
                spriteRender.enabled = false;
            }
            else
            {
                spriteRender.sprite = destroyedSprite;
            }
            /// DISABLE THE GAMEOBJECT
           


            /// DESTROYS THE OBJECT
        }

        IEnumerator DestroyParticles(GameObject psClone)
        {
            ParticleSystem ps =  psClone.GetComponent<ParticleSystem>();
            while (ps.isPlaying)
            {
                yield return null;
            }
            Destroy(psClone);
        }

        public ObjectType GetObjectType()
        {
            return objectType;
        }

        public object CaptureState()
        {
            return null;
        }

        public void RestoreState(object state)
        {
            
            string identifier = GetComponent<SaveableEntity>().GetUniqueIdentifier();
            if (destroyedObjects.Contains(identifier))
            {
                DestroyedState();
                //print(identifier + "  set to destroyed");
            }
        }
    }
}

