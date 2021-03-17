using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Saving;

public class InteractableElements : MonoBehaviour
{
    Collider2D interactableObject;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<IInteractable>() != null)
        {
            print("Entered tent");
            interactableObject = collision;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<IInteractable>() != null)
        {
            interactableObject = null;
        }
    }

    public Collider2D GetInteractableObject()
    {
        print(interactableObject);
        return interactableObject;
    }
}
