using UnityEngine;

namespace PSmash.Control
{
    public class PlayerSpotter : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                //print("PlayerFound");
                transform.parent.GetComponent<EnemyController>().PlayerSpotted(collision.transform, true);
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}

