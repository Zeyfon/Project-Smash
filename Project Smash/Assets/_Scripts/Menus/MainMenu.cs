using UnityEngine;
using PSmash.InputSystem;
using System;

namespace PSmash.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] MainMenuSelector menuSelector = null;
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
            menuSelector.gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            InputHandler.OnPlayerStartButtonPressed += OpenMainMenuViaStartButton;
        }
        private void OnDisable()
        {
            InputHandler.OnPlayerStartButtonPressed -= OpenMainMenuViaStartButton;
        }

        public void CloseMainMenuAndEnablePlayerInput()
        {
            CloseMainMenu();
            if(OnMenuClose != null)
            {
                OnMenuClose();
            }

        }

        public void OpenMainMenuInCraftingSystemSubMenu()
        {
            OpenMenu(SubMenu.CraftingSystem);
        }

        void OpenMainMenuViaStartButton()
        {
            OpenMenu(SubMenu.PlayerStats);
        }

        void OpenMenu(SubMenu subMenu)
        {
            menuSelector.EnableSubMenu(subMenu);
            //SetChildObjects(true);
            _controller.UI.Enable();
            _controller.UI.ButtonStart.started += ctx => CloseMainMenuAndEnablePlayerInput();
        }

        public void CloseMainMenu()
        {
            //SetChildObjects(false);
            _controller.UI.Disable();
            _controller.UI.ButtonStart.started -= ctx => CloseMainMenuAndEnablePlayerInput();
        }
    }


}

