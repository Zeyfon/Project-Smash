﻿using PSmash.Attributes;
using PSmash.Control;
using PSmash.Items.Doors;
using PSmash.Items.Traps;
using PSmash.Movement;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PSmash.InputSystem
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] TimeManager timeManager = null;
        [SerializeField] GameObject weaponsMenu = null;

        public static event Action OnPlayerStartButtonPressed;
        public List<ICommand> commandList = new List<ICommand>();

        ICommand buttonA;
        ICommand buttonB;
        ICommand buttonX;
        ICommand buttonY;
        ICommand buttonRB;
        ICommand buttonLB;
        //ICommand dPadRight;

        PlayerController playerController;
        _Controller _controller;
        PlayerInput input;

        Vector2 movement;

        private void Awake()
        {
            input = GetComponent<PlayerInput>();
            playerController = transform.parent.GetComponent<PlayerController>();
            _controller = new _Controller();
            GameObject.FindObjectOfType<Menus.Menus>()._controller = _controller;
        }
        private void Start()
        {
            SetInitialCommandsToButtons();
            SetCommandList();
            //SetButtonsInControllerMenu();
        }

        private void Update()
        {
            if (!playerController.enabled) return;
            playerController.GetMovement(movement.x, movement.y);
        }

        private void OnEnable()
        {
            _controller.Player.Enable();
            _controller.Player.Move.performed += ctx => movement = ctx.ReadValue<Vector2>();
            _controller.Player.ButtonA.started += ctx => ButtonAPressed();
            _controller.Player.ButtonA.canceled += ctx => ButtonAReleased();
            _controller.Player.ButtonB.started += ctx => ButtonBPressed();
            _controller.Player.ButtonB.canceled += ctx => ButtonBReleased();
            _controller.Player.ButtonX.started += ctx => ButtonXPressed();
            _controller.Player.ButtonX.canceled += ctx => ButtonXReleased();
            _controller.Player.ButtonY.started += ctx => ButtonYPressed();
            _controller.Player.ButtonY.canceled += ctx => ButtonYReleased();
            _controller.Player.ButtonRB.started += cx => ButtonRBPressed();
            _controller.Player.ButtonRB.canceled += ctx => ButtonRBReleased();
            _controller.Player.ButtonLB.started += ctx => ButtonLBPressed();
            _controller.Player.ButtonStart.started += ctx => ButtonStartPressed();
            _controller.Player.Quit.performed += ctx => QuitKeyPressed();
            EventManager.PauseGame += PauseGame;
            EventManager.UnpauseGame += UnpauseGame;
            EventManager.PlayerGotBoots += PlayerGotBoots;
            EventManager.PlayerPerformUncontrolledAction += EnablePlayerController;
            Door.EnablePlayerController += EnablePlayerController;
            PlayerMovement.EnablePlayerController += EnablePlayerController;
            Trap.EnablePlayerController += EnablePlayerController;
            Menus.Menus.OnMenusClosed += EnablePlayerController;
        }

        private void OnDisable()
        {
            _controller.Player.Disable();
            _controller.Player.Move.performed -= ctx => movement = ctx.ReadValue<Vector2>();
            _controller.Player.ButtonA.started -= ctx => ButtonAPressed();
            _controller.Player.ButtonA.canceled -= ctx => ButtonAReleased();
            _controller.Player.ButtonB.started -= ctx => ButtonBPressed();
            _controller.Player.ButtonB.canceled -= ctx => ButtonBReleased();
            _controller.Player.ButtonX.started -= ctx => ButtonXPressed();
            _controller.Player.ButtonX.canceled -= ctx => ButtonXReleased();
            _controller.Player.ButtonY.started -= ctx => ButtonYPressed();
            _controller.Player.ButtonY.canceled -= ctx => ButtonYReleased();
            _controller.Player.ButtonRB.started -= cx => ButtonRBPressed();
            _controller.Player.ButtonRB.canceled -= ctx => ButtonRBReleased();
            _controller.Player.ButtonLB.started -= ctx => ButtonLBPressed();
            _controller.Player.Quit.performed -= ctx => QuitKeyPressed();
            _controller.Player.ButtonStart.started -= ctx => ButtonStartPressed();
            EventManager.PauseGame -= PauseGame;
            EventManager.UnpauseGame -= UnpauseGame;
            EventManager.PlayerGotBoots -= PlayerGotBoots;
            Door.EnablePlayerController -= EnablePlayerController;
            PlayerMovement.EnablePlayerController -= EnablePlayerController;
            Trap.EnablePlayerController -= EnablePlayerController;
            Menus.Menus.OnMenusClosed -= EnablePlayerController;

        }

        private void SetInitialCommandsToButtons()
        {
            buttonA = GetComponent<JumpCommand>();
            buttonX = GetComponent<LightAttackCommand>();
            buttonB = GetComponent<EvadeCommand>();
            buttonY = GetComponent<HeavyAttackCommand>();
            buttonRB = GetComponent<GuardCommand>();
            buttonLB = GetComponent<ThrowableItemsCommand>();
        }

        public Vector2 GetMovement()
        {
            return movement;
        }

        #region ControllerButtons
        private void ButtonXPressed()
        {
            if (buttonX == null) return;
            buttonX.Execute(true);
        }
        private void ButtonXReleased()
        {
            if (buttonX == null) return;
            buttonX.Execute(false);
        }

        private void ButtonBPressed()
        {
            if (buttonB == null) return;
            buttonB.Execute(true);
        }

        private void ButtonBReleased()
        {
            if (buttonB == null) return;
            buttonB.Execute(false);
        }

        private void ButtonAPressed()
        {
            if (buttonA == null) return;
            buttonA.Execute(true);
        }
        private void ButtonAReleased()
        {
            if (buttonA == null) return;
            buttonA.Execute(false);
        }
        private void ButtonYPressed()
        {
            if (buttonY == null) return;
            buttonY.Execute(true);
        }
        private void ButtonYReleased()
        {
            if (buttonY == null) return;
            buttonY.Execute(false);
        }

        private void ButtonRBPressed()
        {
            if (buttonRB == null) return;
            buttonRB.Execute(true);
        }

        private void ButtonRBReleased()
        {
            if (buttonRB == null) return;
            buttonRB.Execute(false);
        }

        private void ButtonLBPressed()
        {
            //print("Want to throw item");
            if (buttonLB == null) return;
            buttonLB.Execute(playerController);
        }
        private void DPadLeftPressed()
        {
            Debug.Log("Pressed");
        }

        private void DPadLeftReleased()
        {
            Debug.Log("Released");
        }

        private void LeftTriggerPressed()
        {
            Debug.Log("Left Trigger Pressed");
            timeManager.SlowTime();
            weaponsMenu.SetActive(true);
        }
        private void LeftTriggerReleased()
        {
            Debug.Log("Left Trigger Released");
            timeManager.SpeedUpTime();
            weaponsMenu.SetActive(false);
        }

        private void ButtonStartPressed()
        {
            if (OnPlayerStartButtonPressed != null)
            {
                //print("Player will open menu");
                //input.currentActionMap = _controller.UI;
                _controller.Player.Disable();
                _controller.UI.Enable();
                OnPlayerStartButtonPressed();
                print(_controller.UI.enabled);
                //_controller.UI.Enable();
                EnablePlayerController(false);
                Time.timeScale = 0;
            }
        }

        private void QuitKeyPressed()
        {
            Debug.Log("Wants to quit");
            Application.Quit();
        }

        #endregion

        //Events coming from Event Manager Object for when Start is pressed
        #region EventManager Events
        void PauseGame()
        {
            print(gameObject.name + "  game was paused");
            movement = new Vector2(0, 0);
            EnablePlayerController(false);
            _controller.UI.Enable();
        }
        void UnpauseGame()
        {
            print(gameObject.name + "  game was unpaused");
            EnablePlayerController(true);
            _controller.UI.Disable();
        }

        void PlayerGotBoots()
        {
            print(gameObject.name + "  game was paused");
            movement = new Vector2(0, 0);
            EnablePlayerController(false);
        }

        void EnablePlayerController(bool state)
        {
            if (!state)
            {
                playerController.SetEnable(false);
                //print("InputHandler Disabled");
            }
            else
            {
                if (!playerController.enabled) playerController.SetEnable(true);
                if (!_controller.Player.enabled) 
                {
                    _controller.Player.Enable();
                    _controller.UI.Disable();
                    Time.timeScale = 1;
                }
                //print("InputHandler Enabled");
            }
        }

        #endregion

        //Here the buttons are set to their initial commands. In a next update the buttons will get 
        //the command to what they where attached in their previous playsesion


        #region Update Button Mapping
        //This list is needed to the Button Change Mechanic 
        void SetCommandList()
        {
            commandList.Add(buttonA);
            commandList.Add(buttonB);
            commandList.Add(buttonX);
            commandList.Add(buttonY);
            commandList.Add(buttonRB);
        }

        //Here you will tell the menu to add the button reference from here to the menu for use there.
        //void SetButtonsInControllerMenu()
        //{
        //    float timer = 0;
        //    while (timer < 0)
        //    {
        //        timer += Time.deltaTime;
        //    }

        //    MenuController menu = GameObject.FindObjectOfType<MenuController>();
        //    if (!menu) return;
        //    menu.BroadcastMessage("GetInitialButton");
        //}

        // The update actions in the Button Map Menu
        //public void SwitchButtonFromMenu(ButtonList button, ActionList newAction)
        //{
        //    DetachCurrentButtonFromThisAction(newAction);
        //    SettingNewButtonToThisAction(button, newAction);
        //}

        //private void DetachCurrentButtonFromThisAction(ActionList newAction)
        //{
        //    switch (newAction)
        //    {
        //        case ActionList.Jump:
        //            foreach (ICommandV2 com in commandList)
        //            {
        //                if (com is JumpCommand)
        //                {
        //                    if (com == buttonA)
        //                    {
        //                        buttonA = null;
        //                        Debug.Log(com + "  will be deleted from ButtonA");
        //                    }
        //                    if (com == buttonB)
        //                    {
        //                        buttonB = null;
        //                        Debug.Log(com + "  will be deleted from ButtonB");
        //                    }
        //                    if (com == buttonX)
        //                    {
        //                        buttonX = null;
        //                        Debug.Log(com + "  will be deleted from ButtonX");
        //                    }
        //                    if (com == buttonY)
        //                    {
        //                        buttonY = null;
        //                        Debug.Log(com + "  will be deleted from ButtonY");
        //                    }
        //                }
        //            }
        //            break;
        //        case ActionList.Attack:
        //            foreach (ICommand com in commandList)
        //            {
        //                if (com is AttackCommand)
        //                {
        //                    if (com == buttonA)
        //                    {
        //                        buttonA = null;
        //                        Debug.Log(com + "  will be deleted from ButtonA");
        //                    }
        //                    if (com == buttonB)
        //                    {
        //                        buttonB = null;
        //                        Debug.Log(com + "  will be deleted from ButtonB");
        //                    }
        //                    if (com == buttonX)
        //                    {
        //                        buttonX = null;
        //                        Debug.Log(com + "  will be deleted from ButtonX");
        //                    }
        //                    if (com == buttonY)
        //                    {
        //                        buttonY = null;
        //                        Debug.Log(com + "  will be deleted from ButtonY");
        //                    }
        //                }
        //            }
        //            break;
        //        case ActionList.Interact:
        //            foreach (ICommand com in commandList)
        //            {
        //                if (com is InteractCommand)
        //                {
        //                    if (com == buttonA)
        //                    {
        //                        buttonA = null;
        //                        Debug.Log(com + "  will be deleted from ButtonA");
        //                    }
        //                    if (com == buttonB)
        //                    {
        //                        buttonB = null;
        //                        Debug.Log(com + "  will be deleted from ButtonB");
        //                    }
        //                    if (com == buttonX)
        //                    {
        //                        buttonX = null;
        //                        Debug.Log(com + "  will be deleted from ButtonX");
        //                    }
        //                    if (com == buttonY)
        //                    {
        //                        buttonY = null;
        //                        Debug.Log(com + "  will be deleted from ButtonY");
        //                    }
        //                }
        //            }
        //            break;
        //    }
        //}
        //private void SettingNewButtonToThisAction(ButtonList button, ActionList newAction)
        //{
        //    print("Switching  " + newAction + "  to  " + button);

        //    switch (button)
        //    {
        //        case ButtonList.buttonA:
        //            switch (newAction)
        //            {
        //                case ActionList.Jump:
        //                    buttonA = GetComponent<JumpCommand>();
        //                    break;
        //                case ActionList.Attack:
        //                    buttonA = GetComponent<AttackCommand>();
        //                    break;
        //                case ActionList.Interact:
        //                    buttonA = GetComponent<InteractCommand>();
        //                    break;
        //                default:
        //                    break;
        //            }
        //            break;
        //        case ButtonList.buttonB:
        //            switch (newAction)
        //            {
        //                case ActionList.Jump:
        //                    buttonB = GetComponent<JumpCommand>();
        //                    break;
        //                case ActionList.Attack:
        //                    buttonB = GetComponent<AttackCommand>();
        //                    break;
        //                case ActionList.Interact:
        //                    buttonB = GetComponent<InteractCommand>();
        //                    break;
        //                default:
        //                    break;
        //            }
        //            break;
        //        case ButtonList.buttonX:
        //            switch (newAction)
        //            {
        //                case ActionList.Jump:
        //                    buttonX = GetComponent<JumpCommand>();
        //                    break;
        //                case ActionList.Attack:
        //                    buttonX = GetComponent<AttackCommand>();
        //                    break;
        //                case ActionList.Interact:
        //                    buttonX = GetComponent<InteractCommand>();
        //                    break;
        //                default:
        //                    break;
        //            }
        //            break;
        //        case ButtonList.buttonY:
        //            switch (newAction)
        //            {
        //                case ActionList.Jump:
        //                    buttonY = GetComponent<JumpCommand>();
        //                    break;
        //                case ActionList.Attack:
        //                    buttonY = GetComponent<AttackCommand>();
        //                    break;
        //                case ActionList.Interact:
        //                    buttonY = GetComponent<InteractCommand>();
        //                    break;
        //                default:
        //                    break;
        //            }
        //            break;
        //        case ButtonList.noButton:
        //            switch (newAction)
        //            {
        //                case ActionList.Jump:
        //                    buttonY = null;
        //                    break;
        //                case ActionList.Attack:
        //                    buttonY = null;
        //                    break;
        //                case ActionList.Interact:
        //                    buttonY = null;
        //                    break;
        //                default:
        //                    break;
        //            }
        //            break;
        //    }
        //}

        #endregion

    }

}