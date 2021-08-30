using PSmash.Attributes;
using PSmash.Inventories;
using PSmash.Items.Doors;
using PSmash.Items.Traps;
using PSmash.Menus;
using System;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using PSmash.SceneManagement;

namespace PSmash.InputSystem
{
    public class InputHandler : MonoBehaviour
    {
        PlayMakerFSM pmPlayerController = null;

        public static event Action OnPlayerStartButtonPressed;
        public List<ICommand> commandList = new List<ICommand>();

        Command action1;
        Command action2;
        Command action3;
        Command action4;
        Command action5;
        Command action6;
        Command action7;
        Command action8;
        Command action9;
        Command action10;
        
        float action5State = 0;
        float action7State = 0;

        FsmObject currentFSM;
        FsmVector2 movementInput;
        _Controller _controller;
        Equipment equipment;

        Vector2 movement;

        private void Awake()
        {
            _controller = new _Controller();
            PlayMakerFSM[] pms = GetComponentsInParent<PlayMakerFSM>();
            equipment = GetComponentInParent<Equipment>();
            foreach(PlayMakerFSM pm in pms)
            {
                if (pm.FsmName == "PlayerController") pmPlayerController = pm;
            }
            if (pmPlayerController == null) Debug.LogWarning("FSM Player Controller could not be found");
        }

        private void Start()
        {
            currentFSM = FsmVariables.GlobalVariables.FindFsmObject("currentFSM");
            movementInput = FsmVariables.GlobalVariables.FindFsmVector2("movementInput");
            SetInitialCommandsToButtons();
        }

        private void SetInitialCommandsToButtons()
        {
            action1 = new JumpCommand();
            action2 = new AttackCommand();
            action3 = new EvadeCommand();
            action4 = new SubweaponCommand();
            action5 = new GuardCommand();
            action6 = new ToolCommand();
            action7 = new ToolSelectionCommand();
            action8 = new GlideCommand();
            action9 = new SubweaponSwitchCommand();
            action10 = new UtilityCommand();
        }

        //Method used by each state in PlayMaker to inform to which state the inputs will be sent
        //public void SetCurrentStateFSM(PlayMakerFSM pm)
        //{
        //    //currentPMState = pm;
        //    //print("Current State in Player is " + currentPMState.FsmName);
        //}

        private void OnEnable()
        {
            _controller.Player.Enable();
            _controller.Player.Action1.started += ctx => Action1Pressed();
            _controller.Player.Action1.canceled += ctx => Action1Released();
            _controller.Player.Action2.started += ctx => Action2Pressed();
            _controller.Player.Action2.canceled += ctx => Action2Released();
            _controller.Player.Action3.started += ctx => Action3Pressed();
            _controller.Player.Action3.canceled += ctx => Action3Released();
            _controller.Player.Action4.started += ctx => Action4Pressed();
            _controller.Player.Action4.canceled += ctx => Action4Released();
            _controller.Player.Action6.started += ctx => Action6Pressed();
            _controller.Player.Action6.canceled += ctx => Action6Released();
            _controller.Player.Action8.started += ctx => Action8Pressed();
            _controller.Player.SubweaponSwitch.started += ctx => SubweaponSwitchPressed();
            _controller.Player.Utility.performed += ctx => UtilityPressed();
            //_controller.Player.SubweaponSwitch.canceled += ctx => SubweaponSwitchPressed();


            _controller.Player.Quit.performed += ctx => QuitKeyPressed();
            _controller.Player.ButtonStart.started += ctx => ButtonStartPressed();

            Mace.onObjectTaken += EnableInput;
            //CraftingSystem.CraftingSystem.OnMenuClose += EnablePlayerInput;
            //MainMenu.OnMenuClose += EnablePlayerInput;
            Menus.Menus.OnMenuClose += EnablePlayerInput;
            //TentMenu.OnTentMenuClose += EnablePlayerInput;
            EventManager.PauseGame += PauseGame;
            EventManager.UnpauseGame += UnpauseGame;
            //EventManager.PlayerGotBoots += PlayerGotBoots;
            EventManager.PlayerPerformUncontrolledAction += IsPlayerInputEnabled;
            Door.EnablePlayerController += IsPlayerInputEnabled;
            Trap.EnablePlayerController += IsPlayerInputEnabled;
            Portal.OnPortalTriggered += EnableInput;
        }

        private void OnDisable()
        {
            _controller.Player.Disable();
            _controller.Player.Action1.started -= ctx => Action1Pressed();
            _controller.Player.Action1.canceled -= ctx => Action1Released();
            _controller.Player.Action2.started -= ctx => Action2Pressed();
            _controller.Player.Action2.canceled -= ctx => Action2Released();
            _controller.Player.Action3.started -= ctx => Action3Pressed();
            _controller.Player.Action3.canceled -= ctx => Action3Released();
            _controller.Player.Action4.started -= ctx => Action4Pressed();
            _controller.Player.Action4.canceled -= ctx => Action4Released();
            _controller.Player.Action6.started -= ctx => Action6Pressed();
            _controller.Player.Action6.canceled -= ctx => Action6Released();
            _controller.Player.Action8.started -= ctx => Action8Pressed();
            _controller.Player.SubweaponSwitch.started -= ctx => SubweaponSwitchPressed();
            _controller.Player.Utility.performed -= ctx => UtilityPressed();
            //_controller.Player.SubweaponSwitch.canceled -= ctx => SubweaponSwitchPressed();

            _controller.Player.Quit.performed -= ctx => QuitKeyPressed();
            _controller.Player.ButtonStart.started -= ctx => ButtonStartPressed();

            Mace.onObjectTaken -= EnableInput;

            //CraftingSystem.CraftingSystem.OnMenuClose -= EnablePlayerInput;
            Menus.Menus.OnMenuClose -= EnablePlayerInput;
            //MainMenu.OnMenuClose -= EnablePlayerInput;
            //TentMenu.OnTentMenuClose -= EnablePlayerInput;
            EventManager.PauseGame -= PauseGame;
            EventManager.UnpauseGame -= UnpauseGame;
            //EventManager.PlayerGotBoots -= PlayerGotBoots;
            Door.EnablePlayerController -= IsPlayerInputEnabled;
            Trap.EnablePlayerController -= IsPlayerInputEnabled;
            Portal.OnPortalTriggered -= EnableInput;
        }

        private void Update()
        {
            action5State = _controller.Player.Action5.ReadValue<float>();
            movement = _controller.Player.Move.ReadValue<Vector2>();
            action7State = _controller.Player.Action7.ReadValue<float>();
            action5.ButtonPressed(transform, this, equipment, currentFSM.Value as PlayMakerFSM, movement, action5State);
            movementInput.Value = movement;
            action7.ButtonPressed(transform, this, equipment, currentFSM.Value as PlayMakerFSM, movement, action7State);
        }

        private void Action1Pressed()
        {
            action1.ButtonPressed(transform,this, equipment, currentFSM.Value as PlayMakerFSM, movement, action5State);
        }
        private void Action1Released()
        {
            action1.ButtonReleased(transform, this, currentFSM.Value as PlayMakerFSM, movement, action5State);
        }


        private void Action2Pressed()
        {
            action2.ButtonPressed(transform, this, equipment, currentFSM.Value as PlayMakerFSM, movement, action5State);
        }
        private void Action2Released()
        {
            print(action2);
            action2.ButtonReleased(transform, this, currentFSM.Value as PlayMakerFSM, movement, action5State);
        }


        private void Action3Pressed()
        {
            action3.ButtonPressed(transform, this, equipment, currentFSM.Value as PlayMakerFSM, movement, action5State);
        }
        private void Action3Released()
        {
            action3.ButtonPressed(transform, this, equipment, currentFSM.Value as PlayMakerFSM, movement, action5State);
        }


        private void Action4Pressed()
        {
            action4.ButtonPressed(transform, this, equipment, currentFSM.Value as PlayMakerFSM, movement, action5State);
        }
        private void Action4Released()
        {
            action4.ButtonReleased(transform, this, currentFSM.Value as PlayMakerFSM, movement, action5State);
        }

        private void Action6Released()
        {
            action6.ButtonReleased(transform, this, currentFSM.Value as PlayMakerFSM, movement, action5State);
        }

        private void Action6Pressed()
        {
            action6.ButtonPressed(transform, this, equipment, currentFSM.Value as PlayMakerFSM, movement, action5State);
        }

        private void Action8Pressed()
        {
            action8.ButtonPressed(transform, this, equipment, currentFSM.Value as PlayMakerFSM, movement, action5State);
        }
        private void SubweaponSwitchPressed()
        {
            action9.ButtonPressed(transform, this, equipment, currentFSM.Value as PlayMakerFSM, movement, action5State);
        }
        private void UtilityPressed()
        {
            action10.ButtonPressed(transform, this, equipment, currentFSM.Value as PlayMakerFSM, movement, action5State);
        }

        private void ButtonStartPressed()
        {
            if (OnPlayerStartButtonPressed != null)
            {
                //print("Player will open menu");
                _controller.Player.Disable();
                OnPlayerStartButtonPressed();
                EnableInput(false);
            }
        }

        private void QuitKeyPressed()
        {
            Debug.Log("Wants to quit");
            Application.Quit();
        }


        //Events coming from Event Manager Object for when Start is pressed
        #region EventManager Events
        void PauseGame()
        {
            print(gameObject.name + "  game was paused");
            movement = new Vector2(0, 0);
            IsPlayerInputEnabled(false);
            //_controller.UI.Enable();
        }
        void UnpauseGame()
        {
            print(gameObject.name + "  game was unpaused");
            IsPlayerInputEnabled(true);
            //_controller.UI.Disable();
        }

        void PlayerGotBoots()
        {
            print(gameObject.name + "  game was paused");
            movement = new Vector2(0, 0);
            IsPlayerInputEnabled(false);
        }

        void IsPlayerInputEnabled(bool isEnabled)
        {
            print("Setting Input Controller");
            if (isEnabled)
            {
                print("Enabling input handler");
                _controller.Player.Enable();
            }
            else
            {
                _controller.Player.Disable();
                print("Disabling input handler");
                //print("InputHandler Enabled");
            }
        }

        public void SetInteractableCollider(Collider2D interactableCollider)
        {
            //this.interactableCollider = interactableCollider;
        }

        void EnablePlayerInput()
        {
            EnableInput(true);
        }

        public void EnableInput(bool isEnabled)
        {
            if (isEnabled)
            {
                print("Enabling input handler");
                _controller.Player.Enable();
            }
            else
            {
                //print("Disabling input handler");
                _controller.Player.Disable();
                movement = new Vector2(0, 0);
            }
        }

        #endregion

        //Here the buttons are set to their initial commands. In a next update the buttons will get 
        //the command to what they where attached in their previous playsesion


        #region Update Button Mapping
        //This list is needed to the Button Change Mechanic 
        //void SetCommandList()
        //{
        //    commandList.Add(action1);
        //    commandList.Add(buttonB);
        //    commandList.Add(buttonX);
        //    commandList.Add(buttonY);
        //    commandList.Add(buttonRB);
        //}

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
