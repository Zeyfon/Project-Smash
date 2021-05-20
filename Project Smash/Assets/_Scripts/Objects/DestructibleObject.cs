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

        int checkpointCounter = 0;
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
            hitsToDestroy--;
            if (hitsToDestroy <=0)
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
            StartCoroutine(DestroyedState());
            GetComponent<ItemDropper>().RandomDrop();
            //string uniqueIdentifier = GetComponent<SaveableEntity>().GetUniqueIdentifier();
            //destroyedObjects.Add(uniqueIdentifier);
        }

        IEnumerator DestroyedState()
        {
            //GetComponent<Collider2D>().enabled = false;
            gameObject.layer = LayerMask.NameToLayer("Dead");
            ///ENABLE A DESTROYED SPRITE
            if (destroyedSprite != null)
            {
                spriteRender.sprite = destroyedSprite;
            }
            else
            {
                spriteRender.enabled = false;
                while (destroyedAudioSource.isPlaying)
                {
                    yield return null;
                }
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.SetActive(false);
                }

            }
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


        //////////////////////////////////////////////////////////SAVE SYSTEM//////////////////////////////////////////////
        [System.Serializable]
        struct Info
        {
            public int checkpointCounter;
            public int hitsToGetDestroyed;
        }

        public object CaptureState()
        {
            print("Capturing state  " + gameObject.name);
            Info info = new Info();
            info.checkpointCounter = FindObjectOfType<WorldManager>().GetCheckpointCounter();
            info.hitsToGetDestroyed = hitsToDestroy;
            return info;
        }

        public void RestoreState(object state, bool isLoadLastScene)
        {
            print("REstoring State  " + gameObject.name);
            Info info = (Info)state;
            WorldManager worldManager = FindObjectOfType<WorldManager>();
            if (worldManager == null)
            {
                Debug.LogWarning("Cannot complete the restore state of this entity");
                return;
            }
            print("1");
            
            checkpointCounter = info.checkpointCounter;
            if (checkpointCounter != worldManager.GetCheckpointCounter())
            {
                print("No overwrite was applied to  " + gameObject.name);
                return;
            }
            else
            {
                print("2");
                hitsToDestroy = info.hitsToGetDestroyed;
                if (hitsToDestroy <= 0)
                {
                    print("Restored to Destroyed State");
                    gameObject.layer = LayerMask.NameToLayer("Dead");
                    for (int i = 0; i < transform.childCount; i++)
                    {
                        transform.GetChild(i).gameObject.SetActive(false);
                    }
                    //DestroyedState();
                }
            }
        }
    }
}

