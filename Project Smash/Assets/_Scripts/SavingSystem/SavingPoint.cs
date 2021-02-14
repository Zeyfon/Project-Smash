using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.InputSystem;
using PSmash.LevelUpSystem;

namespace PSmash.Saving
{
    public class SavingPoint : MonoBehaviour, IInteractable 
    {
        UICraftingSystem craftingSystemMenu;


        // Start is called before the first frame update
        void Awake()
        {
            craftingSystemMenu =  FindObjectOfType<UICraftingSystem>();
           // print(craftingSystemMenu);
        }

        public void StartInteracting()
        {
            print("Player is Interacting with " + gameObject.name);
            StartCoroutine(EnableSavingPointMenu());
        }

        /// <summary>
        /// The menu is enabled with a wait of a frame to avoid the crafting menu from using this submit event
        /// and make the player press the button again to start interacting with the menu.
        /// Everything remains the same.
        /// </summary>
        /// <returns></returns>
        IEnumerator EnableSavingPointMenu()
        {
            yield return new WaitForEndOfFrame();
            craftingSystemMenu.EnableMenu();

        }
        /// <summary>
        /// These Trigger Events works setting the interactableCollider object in the InputHanlder of the player
        /// to perform the necessary action depending on the object the player will interact to
        /// </summary>
        /// <param name="collision"></param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collision.transform.GetChild(0).GetComponent<InputHandler>().SetInteractableCollider(GetComponent<Collider2D>());
                GetComponent<IShowInputPrompt>().ShowInputPrompt();
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collision.transform.GetChild(0).GetComponent<InputHandler>().SetInteractableCollider(null);
                GetComponent<IHideInputPrompt>().HideInputPrompt();
            }
        }
    }

}
