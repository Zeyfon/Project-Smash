using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Items;

namespace PSmash.Control
{
    public class PlayerInteractions : MonoBehaviour
    {
        [SerializeField] float pushableForce = 4500;
        Coroutine coroutine;

        public void MovingObject(Transform interactableObjectTransform)
        {
            float xOffset = interactableObjectTransform.GetComponent<InteractableObject>().xOffset;
            float objectRelativePosition = interactableObjectTransform.position.x - transform.position.x;
            if (objectRelativePosition >= 0)
            {
                print("Player is Left");
            }
            else
            {
                print("Player is Right");
                xOffset *= -1;
            }
            coroutine = StartCoroutine(MovingObject2(interactableObjectTransform, xOffset));
        }

        public void StopInteracting(Transform interactableObjectTransform)
        {
            StopCoroutine(coroutine);
        }

        IEnumerator MovingObject2(Transform interactableObjectTransform, float xOffset)
        {

            Rigidbody2D rb = interactableObjectTransform.GetComponent<Rigidbody2D>();
            while (true)
            {
                yield return new WaitForFixedUpdate();
                rb.MovePosition(new Vector2(transform.position.x + xOffset, interactableObjectTransform.position.y));
                //print("Object " + (Vector2)interactableObjectTransform.position);
            }
        }

    }
}

