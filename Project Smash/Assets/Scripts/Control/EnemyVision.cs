using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Events;

namespace PSmash.Control
{
    

    public class EnemyVision : MonoBehaviour
    {
        [SerializeField] UnityEvent onPlayerSpotted;

        Transform target;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                //print(transform.parent.gameObject.name + "  PlayerSpotted ");
                onPlayerSpotted.Invoke();
            }
        }
    }
}

