using UnityEngine;
using PSmash.InputSystem;
using System;

namespace PSmash.Menus
{
    /// <summary>
    /// Its only function is to Instantiante and keep organize all the menus the game will use.
    /// </summary>
    public class Menus : MonoBehaviour
    {
        [SerializeField] GameObject status = null;        
        [SerializeField] GameObject tentMenu = null;


        public static event Action OnMenuClose;
        _Controller _controller;

        void Awake()
        {
            _controller = new _Controller();
            Instantiate(status, transform);
            Instantiate(tentMenu, transform);
        }
        private void OnEnable()
        {
            InputHandler.OnPlayerStartButtonPressed += OpenMainMenuViaStartButton;
            ContinueButton.OnMenuClose += CloseMenu;
        }
        private void OnDisable()
        {
            InputHandler.OnPlayerStartButtonPressed -= OpenMainMenuViaStartButton;
            ContinueButton.OnMenuClose -= CloseMenu;
        }

        private void CloseMenu()
        {
            _controller.UI.Disable();
            _controller.UI.Cancel.started -= ctx => CloseMenu();
            print("Close Menu");
            foreach(Canvas canvas in GetComponentsInChildren<Canvas>())
            {
                canvas.transform.GetChild(0).gameObject.SetActive(false);
            }
            OnMenuClose();
        }

        private void OpenMainMenuViaStartButton()
        {
            _controller.UI.Enable();
            _controller.UI.Cancel.started += ctx => CloseMenu();
            GetComponentInChildren<MainMenu>().OpenMenu(SubMenu.PlayerStats);
        }
    }
}

