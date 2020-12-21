using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using PSmash.Attributes;

namespace PSmash.Menus
{
    public class GameMenu : MonoBehaviour
    {
        [SerializeField] _Controller _controller;
        public bool menuIsOpened = false;
        PlayerInput playerInput;
        EventManager eventManager;
        bool isMenuOpen = false;
        bool canInteractWithMenu = true;
        private void Awake()
        {
            eventManager = FindObjectOfType<EventManager>();
            playerInput = FindObjectOfType<PlayerInput>();
        }
        private void Start()
        {
            Transform transform1 = transform.GetChild(0);
        }
        private void OnEnable()
        {
            EventManager.StartButtonPressed += StartButtonPressed;

        }

        private void OnDisable()
        {
            EventManager.StartButtonPressed -= StartButtonPressed;
        }

        private void StartButtonPressed()
        {
            if (!isMenuOpen && canInteractWithMenu)
            {
                eventManager.GameWillPause();
                transform.GetChild(0).gameObject.SetActive(true);
                isMenuOpen = true;
            }
            else if (isMenuOpen && canInteractWithMenu)
            {
                eventManager.GameWillUnpause();
                transform.GetChild(0).gameObject.SetActive(false);
                isMenuOpen = false;
            }
            else
            {
                //Do Nothing for now
            }
        }
    }
}

