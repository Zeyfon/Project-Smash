using PSmash.Inventories;
using PSmash.Menus;
using PSmash.SceneManagement;
using System;
using UnityEngine;

namespace PSmash.CraftingSystem
{
    public class CraftingSystemMenuManagement : MonoBehaviour
    {
        //STATE
        public static event Action OnMenuClose;

        _Controller _controller;
        TentMenu tentMenu;
        //INITIALIZE
        private void Awake()
        {
            _controller = new _Controller();
        }

        // Start is called before the first frame update
        void Start()
        {
            tentMenu = FindObjectOfType<TentMenu>();
            if (tentMenu == null)
                Debug.LogWarning("No Tent Menu Found");
            CloseMenu();
        }

        ///////////////////////////////////////MENU MANAGEMENENT///////////////////

        private void OnEnable()
        {
            TentMenu.OnNextMenuOpen += OpenMenu;
        }

        private void OnDisable()
        {
            TentMenu.OnNextMenuOpen -= OpenMenu;
        }

        void OpenMenu()
        {
            SetChildObjects(true);
            GetComponentInChildren<CraftingSystem>().UpdatePanel(null);
            _controller.UI.Enable();
            _controller.UI.Cancel.performed += ctx => BacktrackMenu();
            _controller.UI.ButtonStart.performed += ctx => CloseAllMenus();
        }

        private void CloseAllMenus()
        {
            if (OnMenuClose != null)
            {
                OnMenuClose();
            }
            CloseMenu();
            FindObjectOfType<SavingWrapper>().Save();
            _controller.UI.Cancel.performed -= ctx => BacktrackMenu();
            _controller.UI.ButtonStart.performed -= ctx => CloseAllMenus();
            _controller.UI.Disable();
        }

        void BacktrackMenu()
        {
            //print("Backtracking Menu ");
            CloseMenu();
            FindObjectOfType<SavingWrapper>().Save();
            tentMenu.OpenTentMenuAndDoCheckpoint();
            _controller.UI.Cancel.performed -= ctx => BacktrackMenu();
            _controller.UI.ButtonStart.performed -= ctx => CloseAllMenus();
            _controller.UI.Disable();
        }

        void CloseMenu()
        {
            SetChildObjects(false);
        }

        void SetChildObjects(bool isEnabled)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(isEnabled);
            }
        }

    }

}
