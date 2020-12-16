using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Core
{
    public class BeatenUpMomentTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                transform.parent.GetComponent<BeatenUpAreaController>().StartBeatenUpMoment();
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}

