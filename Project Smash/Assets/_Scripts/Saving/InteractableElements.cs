using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Core
{
    public class InteractableElements : MonoBehaviour
    {
        IManualInteraction manualInteractableObject;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collision.CompareTag("Interactable"))
                return;
            //print(collision.gameObject.name +  "  is trying to interact with the player");
            if (collision.GetComponent<IManualInteraction>() != null)
            {
                manualInteractableObject = collision.GetComponent<IManualInteraction>();
            }
            else if(collision.GetComponentInParent<IAutomaticInteraction>()!= null)
            {
                //print(collision.gameObject.name);
                collision.GetComponentInParent<IAutomaticInteraction>().Interact();
            }
            else
            {
                Debug.LogWarning("No interaction was possible with interactable element");
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<IManualInteraction>() != null)
            {
                manualInteractableObject = null;
            }
        }

        public IManualInteraction GetInteractableObject()
        {
            //print(interactableObject);
            return manualInteractableObject;
        }
    }

}
