using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;
using PSmash.Combat.Weapons;
using PSmash.Combat;
using PSmash.Inventories;
using GameDevTV.Saving;
using PSmash.Checkpoints;

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

        static int checkpointCounter = 0;
        public static List<string> destroyedObjects = new List<string>();

        public enum ObjectType
        {
            barrel,
            crate,
            rock
        }

        int counter = 0;

        public void TakeDamage(Transform attacker, Weapon weapon, AttackType attackType, float damage, float attackForce)
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

        [System.Serializable]
        struct Info
        {
            public int checkpointCounter;
        }

        public object CaptureState()
        {
            WorldManager worldManager = FindObjectOfType<WorldManager>();
            if (worldManager == null)
                return null;

            Info info = new Info();
            info.checkpointCounter = worldManager.GetCheckpointCounter();
            return info;
        }

        public void RestoreState(object state, bool isLoadLastScene)
        {
            if (state == null || isLoadLastScene)
                return;

            WorldManager worldManager = FindObjectOfType<WorldManager>();
            if (worldManager == null)
                return;

            Info info = (Info)state;
            if (info.checkpointCounter == worldManager.GetCheckpointCounter())
            {
                checkpointCounter = worldManager.GetCheckpointCounter();
                string identifier = GetComponent<SaveableEntity>().GetUniqueIdentifier();
                if (destroyedObjects.Contains(identifier))
                {
                    DestroyedState();
                }
            }
        }
    }
}

