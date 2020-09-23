using PSmash.Core;
using System;
using System.Collections;
using UnityEngine;

namespace PSmash.Menus
{
    public class Menus : MonoBehaviour
    {
        [SerializeField] Transform weaponsTransform = null;
        [SerializeField] Transform statusTransform = null;
        public delegate void MenusClosed(bool state);
        public static event MenusClosed OnMenusClosed;

        _Controller _controller;
        // Start is called before the first frame update
        void Awake()
        {
            _controller = new _Controller();
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
            statusTransform.gameObject.SetActive(true);
            _controller.UI.Enable();
            print("Menu activated");
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
            statusTransform.GetComponent<Status>().StarCollected();
        }

        private void CloseMenus()
        {
            statusTransform.gameObject.SetActive(false);
            _controller.UI.Disable();
            OnMenusClosed(true);
        }
    }
}

