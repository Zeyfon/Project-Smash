using UnityEngine;
using PSmash.InputSystem;
using System;
using PSmash.Checkpoints;
using PSmash.SceneManagement;

namespace PSmash.Menus
{
    /// <summary>
    /// Its only function is to Instantiante and keep organize all the menus the game will use.
    /// </summary>
    public class Menus : MonoBehaviour
    {
        [SerializeField] GameObject status = null;        
       // [SerializeField] GameObject tentMenu = null;

        public static event Action OnMenuClose;

        _Controller _controller;

        bool saveWhenExit = false;


        void Awake()
        {
            _controller = new _Controller();
            Instantiate(status, transform);
            //Instantiate(tentMenu, transform);
        }
        private void OnEnable()
        {
            InputHandler.OnPlayerStartButtonPressed += OpenMainMenuViaStartButton;
            ContinueButton.OnMenuClose += CloseMainMenu;
            Tent.OnTentMenuOpen += OpenMenuViaCraftingSystem;
        }
        private void OnDisable()
        {
            InputHandler.OnPlayerStartButtonPressed -= OpenMainMenuViaStartButton;
            ContinueButton.OnMenuClose -= CloseMainMenu;
            Tent.OnTentMenuOpen -= OpenMenuViaCraftingSystem;

        }

        private void OpenMainMenuViaStartButton()
        {
            CloseMenusBeforeOpenOne();
            _controller.UI.Enable();
            _controller.UI.Cancel.started += ctx => CloseMainMenu();
            GetComponentInChildren<MainMenu>().OpenMenu(SubMenu.PlayerStats);
            saveWhenExit = false;
        }

        public void OpenMenuViaCraftingSystem()
        {
            CloseMenusBeforeOpenOne();
            _controller.UI.Enable();
            _controller.UI.Cancel.started += ctx => CloseMainMenu();
            GetComponentInChildren<MainMenu>().OpenMenu(SubMenu.CraftingSystem);
            saveWhenExit = true;
        }

        public void CloseMenusBeforeOpenOne()
        {
            foreach (Canvas canvas in GetComponentsInChildren<Canvas>())
            {
                canvas.transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        public void CloseMainMenu()
        {
            if (saveWhenExit)
            {
                saveWhenExit = false;
                FindObjectOfType<SavingWrapper>().Save();
            }
            _controller.UI.Disable();
            _controller.UI.Cancel.started -= ctx => CloseMainMenu();
            print("Close Menu");
            foreach (Canvas canvas in GetComponentsInChildren<Canvas>())
            {
                canvas.transform.GetChild(0).gameObject.SetActive(false);
            }
            OnMenuClose();
        }
    }
}

