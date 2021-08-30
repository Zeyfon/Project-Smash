using UnityEngine;
using PSmash.InputSystem;
using System;

namespace PSmash.Menus
{
    public class MainMenu : MonoBehaviour
    {
        void Start()
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }

        /// <summary>
        /// Opens the main menu in the SubMenu passed in the paremeters
        /// </summary>
        /// <param name="subMenu"></param>
        public void OpenMenu(SubMenu subMenu)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            GetComponentInChildren<MainMenuSelector>().EnableSubMenu(subMenu);
        }
    }


}

