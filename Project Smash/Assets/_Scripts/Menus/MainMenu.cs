using UnityEngine;
using PSmash.InputSystem;
using System;

namespace PSmash.Menus
{
    public class MainMenu : MonoBehaviour
    {
        // Start is called before the first frame update
        _Controller _controller;

        public delegate void MainMenuAction(bool isEnabled);
        public static event MainMenuAction OnMenuAction;

        public static event Action OnMenuClose;

        private void Awake()
        {
            _controller = new _Controller();
        }
        void Start()
        {
            SetChildObjects(false);
        }

        private void OnEnable()
        {
            InputHandler.OnPlayerStartButtonPressed += OpenMainMenu;
        }
        private void OnDisable()
        {
            InputHandler.OnPlayerStartButtonPressed -= OpenMainMenu;
        }

        public void CloseMainMenuAndEnablePlayerInput()
        {
            CloseMainMenu();
            if(OnMenuClose != null)
            {
                OnMenuClose();
            }

        }

        void OpenMainMenu()
        {

            OpenMenu();
        }

        void OpenMenu()
        {
            SetChildObjects(true);
            _controller.UI.Enable();
            _controller.UI.ButtonStart.started += ctx => CloseMainMenuAndEnablePlayerInput();
        }

        public void CloseMainMenu()
        {
            SetChildObjects(false);
            _controller.UI.Disable();
            _controller.UI.ButtonStart.started -= ctx => CloseMainMenuAndEnablePlayerInput();
        }

        private void SetChildObjects(bool isEnabled)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(isEnabled);
            }
        }
    }


}

