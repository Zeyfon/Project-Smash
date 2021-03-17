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

        public void CloseMainMenu()
        {
            CloseMenu();
            if(OnMenuClose != null)
            {
                OnMenuClose();
            }
            _controller.UI.Disable();
            _controller.UI.ButtonStart.started -= ctx => CloseMainMenu();
        }

        void OpenMainMenu()
        {
            _controller.UI.Enable();
            _controller.UI.ButtonStart.started += ctx => CloseMainMenu();
            OpenMenu();
        }

        void OpenMenu()
        {
            SetChildObjects(true);
        }

        void CloseMenu()
        {
            SetChildObjects(false);
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

