using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{
    public class ReturnPosition : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                transform.parent.GetComponent<IReturnPosition>().SetNewPosition(transform.position);
            }
        }
    }
}

