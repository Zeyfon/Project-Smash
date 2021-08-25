using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using PSmash.InputSystem;

namespace PSmash.Menus
{
    public class MenuController : MonoBehaviour
    {
        [SerializeField] Transform buttonTransform = null;
        [SerializeField] Transform weaponsTransform = null;
        [SerializeField] Transform statusTransform = null;
        [SerializeField] List<InputButton> inputButtons = new List<InputButton>();

        InputButton inputButton;
        //_Controller _controller;
        EventSystem eventSystem;

        int childrenCounter = 0;
        // Start is called before the first frame update
        void Awake()
        {
            //_controller = new _Controller();
            eventSystem = FindObjectOfType<EventSystem>();
        }
        void Update()
        {
            if (buttonTransform != GetCurrentButton())
            {
                buttonTransform = GetCurrentButton();
                inputButton = buttonTransform.GetComponent<InputButton>();
                //print(inputButton.name);
            }
        }

        Transform GetCurrentButton()
        {
            return eventSystem.currentSelectedGameObject.transform;
        }

        private void OnEnable()
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(buttonTransform.gameObject);
            //_controller.UI.ButtonA.performed += ButtonAPressed;
            //_controller.UI.ButtonB.performed += ButtonBPressed;
            //_controller.UI.ButtonX.performed += ButtonXPressed;
            //_controller.UI.ButtonY.performed += ButtonYPressed;
        }

        void EnableMenus()
        {

        }

        private void OnDisable()
        {
            //_controller.UI.ButtonA.performed -= ButtonAPressed;
            //_controller.UI.ButtonB.performed -= ButtonBPressed;
            //_controller.UI.ButtonX.performed -= ButtonXPressed;
            //_controller.UI.ButtonY.performed -= ButtonYPressed;
        }

        private void ButtonAPressed(InputAction.CallbackContext obj)
        {
            SetNewButtonLayout(ButtonList.buttonA);
        }
        //Events attached to the Player Input in the Controller gameObject of the Player

        private void ButtonBPressed(InputAction.CallbackContext obj)
        {
            //SetNewButtonLayout(ButtonList.buttonB);

        }
        private void ButtonXPressed(InputAction.CallbackContext obj)
        {
            SetNewButtonLayout(ButtonList.buttonX);
        }
        private void ButtonYPressed(InputAction.CallbackContext obj)
        {
            SetNewButtonLayout(ButtonList.buttonY);
        }

        void SetNewButtonLayout(ButtonList newButton)
        {
            //print("Set " + inputButton.gameObject.name + " to  " + newButton);
            inputButton.SetButton(newButton);
            DeleteThisButtonFromOtherActions(newButton);
        }

        private void DeleteThisButtonFromOtherActions(ButtonList newButton)
        {
            foreach (InputButton tempInputButton in inputButtons)
            {
                //("The current input to evaluate is " + tempInputButton);
                if (tempInputButton == inputButton) continue;
                //print(newButton + "  " + tempInputButton.myButton);
                if (newButton == tempInputButton.myButton)
                {
                    //print(newButton + "  will be deleted from  " + tempInputButton.gameObject.name);
                    tempInputButton.SetButton(ButtonList.noButton);
                }
            }
        }

        public void InitialChildrenSet()
        {
            InputButton[] controllerButtons = GetComponentsInChildren<InputButton>();
            childrenCounter++;
            if (childrenCounter == controllerButtons.Length) transform.gameObject.SetActive(false);
        }
    }

}
