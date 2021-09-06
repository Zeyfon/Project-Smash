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
        // Start is called before the first frame update
        void Start()
        {
            DisableMenuObjects();
        }
        //private void OnEnable()
        //{
        //    Tent.OnTentMenuOpen += OpenTentMenuAndDoCheckpoint;
        //}

        //private void OnDisable()
        //{
        //    Tent.OnTentMenuOpen -= OpenTentMenuAndDoCheckpoint;
        //}

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
            TentEnterSound();
        }

        /// <summary>
        /// Opens the Tent Menu without doing a Checkpoint again
        /// </summary>
        public void OpenMenu()
        {
            //print("Open Tent Menu");
            transform.GetChild(0).gameObject.SetActive(true);
            //SetChildObjects(true);
            //StartCoroutine(EnableControl());
            StartCoroutine(InitializeSelection());
        }

        ////////INPUT/////////

        /// <summary>
        /// Will Close all menus and enable player controller
        /// </summary>
        private void CloseAllMenus()
        {
            //print("Close All Menus");
            CloseTentMenu();
        }

        /// <summary>
        /// Will go back to the previous menu. If no previous is available will close the menu and enable player control
        /// </summary>
        private void BacktrackMenu()
        {
            //print("BackTracking in Menus");
            CloseTentMenu();
        }



        /// <summary>
        /// TODO. This part must be taken out as is a method used by the button in the Tent Menu to open the Crafting Menu
        /// </summary>
        public void OpenMainMenuInCraftingSystemSubMenu()
        {
            transform.parent.parent.GetComponentInChildren<Menus>().OpenMenuViaCraftingSystem();
        }

        /////////////PRIVATE///////////////

        void TentEnterSound()
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
            //EnableUIController();
        }

        void CloseTentMenu()
        {
            DisableMenuObjects();
            if(OnTentMenuClose == null)
            {
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


        void SetChildObjects(bool isEnabled)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(isEnabled);
            }
        }


    }

}

