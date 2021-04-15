using System.Collections;
using UnityEngine;
using PSmash.Combat;
using PSmash.Attributes;
using PSmash.Combat.Weapons;

namespace PSmash.Temporal
{
    public class BombFire : MonoBehaviour
    {
        [SerializeField] int damage = 2;
        [SerializeField] float damageTime = 5;
        [SerializeField] float damageInterval = 0.5f;
        [SerializeField] LayerMask whatIsAttackable;

        [SerializeField] float fadeInTime = 0.5f;
        AudioSource audioSource;
        float timer = 0;
        Weapon weapon;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(FireDamageToEnemies());
            audioSource = GetComponent<AudioSource>();
            StartCoroutine(AudioFadeIn());
            StartCoroutine(FireDamageToItems());
        }

        IEnumerator AudioFadeIn()
        {
            float volume = 0;
            while (volume < 1)
            {
                volume += Time.deltaTime * fadeInTime;
                if (volume >= 0) volume = 1;
                audioSource.volume = volume;
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator FireDamageToEnemies()
        {
            EnemyHealth target;
            float timer = 0;
            Vector2 size = GetComponent<BoxCollider2D>().size;
            Vector2 origin = GetComponent<BoxCollider2D>().offset + (Vector2)transform.position;

            while (timer < damageTime)
            {
                Collider2D[] colls = Physics2D.OverlapBoxAll(origin, size, whatIsAttackable);
                foreach (Collider2D coll in colls)
                {
                    target = coll.GetComponent<EnemyHealth>();
                    if (target != null) coll.GetComponent<IDamagable>().TakeDamage(null, weapon, AttackType.NotUnblockable,damage);
                }
                timer += damageInterval;
                yield return new WaitForSeconds(damageInterval);
            }
            Destroy(gameObject);
        }

        IEnumerator FireDamageToItems()
        {
            ItemsHealth target;
            Vector2 size = GetComponent<BoxCollider2D>().size;
            Vector2 origin = GetComponent<BoxCollider2D>().offset + (Vector2)transform.position;
            Collider2D[] colls = Physics2D.OverlapBoxAll(origin, size, whatIsAttackable);
            foreach (Collider2D coll in colls)
            {
                target = coll.GetComponent<ItemsHealth>();
                if (target != null) coll.GetComponent<ItemsHealth>().Burn();
            }
            yield return null;
        }
    }
}

