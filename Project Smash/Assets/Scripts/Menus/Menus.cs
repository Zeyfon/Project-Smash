using PSmash.Core;
using System;
using System.Collections;
using UnityEngine;
using PSmash.InputSystem;

namespace PSmash.Menus
{
    public class Menus : MonoBehaviour
    {
        //[SerializeField] Transform weaponsTransform = null;
        [SerializeField] GameObject status = null;
        public delegate void MenusClosed(bool state);
        public static event MenusClosed OnMenusClosed;
        GameObject statusMenuClone;
        public _Controller _controller;
        // Start is called before the first frame update
        void Awake()
        {
            statusMenuClone = Instantiate(status, transform);
        }

        private void OnEnable()
        {
            _controller.UI.ButtonB.performed += ctx => ButtonBPressed();
            _controller.UI.ButtonStart.performed += ctx => ButtonStartPressed();
            PSmash.InputSystem.InputHandlerV2.OnPlayerStartButtonPressed += EnableMenus;
            PSmash.Items.Star.OnStarCollected += StarCollected;
        }

        private void OnDisable()
        {
            _controller.UI.ButtonB.performed -= ctx => ButtonBPressed();
            _controller.UI.ButtonStart.performed -= ctx => ButtonStartPressed();
            PSmash.InputSystem.InputHandlerV2.OnPlayerStartButtonPressed -= EnableMenus;
            PSmash.Items.Star.OnStarCollected -= StarCollected;
        }

        void EnableMenus()
        {
            statusMenuClone.SetActive(true);
        }

        void ButtonStartPressed()
        {
            CloseMenus();
        }

        void ButtonBPressed()
        {
            CloseMenus();
        }

        void StarCollected()
        {
            statusMenuClone.GetComponent<MainMenu>().StarCollected();
        }

        public  void CloseMenus()
        {
            //print("Closing Menu");

            statusMenuClone.SetActive(false);
            OnMenusClosed(true);
        }
    }
}

