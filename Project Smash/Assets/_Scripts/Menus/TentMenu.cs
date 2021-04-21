using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Checkpoints;
using System;
using UnityEngine.EventSystems;

namespace PSmash.Menus
{
    public class TentMenu : MonoBehaviour
    {
        [SerializeField] GameObject initialSelection = null;
        [SerializeField] AudioSource CheckpointOpenedAudioSource = null;

        public static event Action OnNextMenuOpen;

        public static event Action OnTentMenuClose;
        _Controller _controller;
        // Start is called before the first frame update
        void Start()
        {
            _controller = new _Controller();
            DisableMenuObjects();
        }
        private void OnEnable()
        {
            Tent.OnTentMenuOpen += OpenTentMenuAndDoCheckpoint;
        }

        private void OnDisable()
        {
            Tent.OnTentMenuOpen -= OpenTentMenuAndDoCheckpoint;
        }

        ///////////PUBLIC////////////// 
        ///
        /// <summary>
        /// The method called by the event to open the Tent Menu.
        /// The signal comes from the InputHandler inside the Player for now.
        /// </summary>

        public void OpenTentMenuAndDoCheckpoint()
        {
            OpenMenu();
            //print(OnTentMenuClose.Target);
            Checkpoint();
        }

        /// <summary>
        /// Opens the Tent Menu without doing a Checkpoint again
        /// </summary>
        public void OpenMenu()
        {
            //print("Open Tent Menu");
            SetChildObjects(true);
            StartCoroutine(EnableControl());
            StartCoroutine(InitializeSelection());
        }

        /////////////PRIVATE///////////////

        private void Checkpoint()
        {
            CheckpointOpenedAudioSource.Play();
        }

        IEnumerator InitializeSelection()
        {
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            eventSystem.SetSelectedGameObject(null);
            yield return null;
            eventSystem.SetSelectedGameObject(initialSelection);
        }

        IEnumerator EnableControl()
        {
            yield return new WaitForEndOfFrame();
            EnableUIController();
        }

        void CloseTentMenu()
        {
            //print("Closing Tent Menu");
            DisableUIController();
            DisableMenuObjects();
            if(OnTentMenuClose == null)
            {
                Debug.LogWarning("Player will not enable controller again");
                return;
            }
                OnTentMenuClose();
        }

        /// <summary>
        /// Open and Close menu actions
        /// </summary>

        void DisableMenuObjects()
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


        private void EnableUIController()
        {
            //print("Enable Tent Menu Controller");
            _controller.UI.Enable();
            _controller.UI.Cancel.performed += ctx => BacktrackMenu();
            _controller.UI.ButtonStart.performed += ctx => CloseAllMenus();
        }

        private void DisableUIController()
        {
            //print("Disable Tent Menu Controller");
            _controller.UI.Disable();
            _controller.UI.Cancel.performed -= ctx => BacktrackMenu();
            _controller.UI.ButtonStart.performed -= ctx => CloseAllMenus();
        }

        ////////INPUT/////////
        
        /// <summary>
        /// Will Close all menus and enable player controller
        /// </summary>
        private void CloseAllMenus()
        {
            CloseTentMenu();
        }

        /// <summary>
        /// Will go back to the previous menu. If no previous is available will close the menu and enable player control
        /// </summary>
        private void BacktrackMenu()
        {
            CloseTentMenu();
        }


        /// <summary>
        /// TODO. This part must be taken out as is a method used by the button in the Tent Menu to open the Crafting Menu
        /// </summary>
        public void OpenCraftingMenu()
        {
            if (OnNextMenuOpen != null)
            {
                OnNextMenuOpen();
                DisableMenuObjects();
                DisableUIController();
            }
        }
    }

}

