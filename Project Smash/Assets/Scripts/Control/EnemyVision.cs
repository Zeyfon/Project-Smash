using System.Xml.Schema;
using UnityEngine;

namespace PSmash.Control
{
    public class EnemyVision : MonoBehaviour
    {

        Transform target;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                target = collision.transform;
            }
        }

        public Transform Target
        {
            get
            {
                return target;
            }
            set
            {
                target = value;
            }
        }
    }
}

