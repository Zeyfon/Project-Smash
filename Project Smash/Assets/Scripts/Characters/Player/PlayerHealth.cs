using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PSmash.Combat
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField] int health = 100;
        HealthBar healhtBar;
        EventManager eventManager;

        bool isDead = false;
        bool isGuarding = true;
        int initialHealth;
        int initialGuard;
        // Start is called before the first frame update
        void Start()
        {
            eventManager = FindObjectOfType<EventManager>();
            initialHealth = health;
        }

        public bool IsDead()
        {
            return isDead;
        }

        public void SendDamage(Vector2 attackerPosition, int damage)
        {
            if (isDead) return;
            //Debug.Log(gameObject.name + "  was hit");
            if (transform.CompareTag("Enemy") || transform.CompareTag("Player"))
            {
                //Debug.Log(transform.gameObject.name + "  " +  damage);
                Damaged(attackerPosition, damage);
            }
            if (transform.CompareTag("InteractableObject"))
            {
                GetComponent<InteractableObject>().ApplyImpulseForce(attackerPosition, damage);
            }
        }

        void Damaged(Vector2 attackerPosition, int damage) 
        {
            float healthScale;
            health -= damage;
            if (health <= 0)
            {
                healthScale = 0;
                isDead = true;
                //Debug.Log(gameObject.name + " dead");
                StartCoroutine(GameObjectDied());
            }
            else
            {
                healthScale = (float)health / (float)initialHealth;
            }
            eventManager.PlayerReceivedDamage(healthScale);
            //Debug.Break();
        }

        IEnumerator GameObjectDied()
        {
            yield return new WaitForSeconds(2);
            //Destroy(gameObject);
            SceneManager.LoadScene(0);
        }
    }
}


