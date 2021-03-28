using PSmash.Attributes;
using PSmash.Items.Doors;
using PSmash.Items.Traps;
using PSmash.Inventories;
using PSmash.Movement;
using System;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Saving;
using PSmash.Menus;
using PSmash.CraftingSystem;
using PSmash.Core;

namespace PSmash.InputSystem
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] PlayerMovement pMovement = null;
        [SerializeField] Equipment equipment = null;

        Collider2D interactableCollider;
        PlayMakerFSM pmPlayerController = null;


        public static event Action OnPlayerStartButtonPressed;
        public List<ICommand> commandList = new List<ICommand>();

        ICommand action1;
        ICommand buttonB;
        ICommand buttonX;
        ICommand buttonY;
        ICommand buttonRB;
        ICommand buttonLB;
        //ICommand dPadRight;

        PlayMakerFSM currentPMState;
        _Controller _controller;

        Vector2 movement;
        float itemSelect;
        bool jumpButtonState = false;
        bool toolButtonState = false;
        bool guardButtonState = false;

        private void Awake()
        {
            _controller = new _Controller();
            PlayMakerFSM[] pms = GetComponentsInParent<PlayMakerFSM>();
            foreach(PlayMakerFSM pm in pms)
            {
                if (pm.FsmName == "PlayerController") pmPlayerController = pm;
            }
            if (pmPlayerController == null) Debug.LogWarning("FSM Player Controller could not be found");
            //playerController = transform.parent.GetComponent<PlayerController>();
        }
        private void Start()
        {
            SetInitialCommandsToButtons();
            //SetCommandList();
            //SetButtonsInControllerMenu();
        }

        private void SetInitialCommandsToButtons()
        {
            action1 = GetComponent<JumpCommand>();
            buttonX = GetComponent<LightAttackCommand>();
            buttonB = GetComponent<EvadeCommand>();
            buttonY = GetComponent<ToolCommand>();
            buttonRB = GetComponent<GuardCommand>();
            buttonLB = GetComponent<ThrowableItemsCommand>();
        }

        public _Controller GetController()
        {
            return _controller;
        }

        //Method used by each state in PlayMaker to inform to which state the inputs will be sent
        public void SetCurrentStateFSM(PlayMakerFSM pm)
        {
            currentPMState = pm;
            //print("Current State in Player is " + currentPMState.FsmName);
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
            _controller.Player.ItemUse.started += ctx => ItemButtonPressed();
            _controller.Player.ItemUse.canceled += ctx => ItemButonReleased();
            _controller.Player.ItemChangeRight.performed += ctx => ItemChangeRight();
            _controller.Player.ItemChangeLeft.performed += ctx => ItemChangeLeft();
            _controller.Player.ButtonStart.started += ctx => ButtonStartPressed();

            CraftingSystem.CraftingSystem.OnMenuClose += EnablePlayerInput;
            MainMenu.OnMenuClose += EnablePlayerInput;
            TentMenu.OnTentMenuClose += EnablePlayerInput;
            EventManager.PauseGame += PauseGame;
            EventManager.UnpauseGame += UnpauseGame;
            EventManager.PlayerGotBoots += PlayerGotBoots;
            EventManager.PlayerPerformUncontrolledAction += IsPlayerInputEnabled;
            Door.EnablePlayerController += IsPlayerInputEnabled;
            Trap.EnablePlayerController += IsPlayerInputEnabled;
            MainMenu.OnMenuAction += IsPlayerInputEnabled;
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
          //  _controller.Player.ButtonLB.started -= ctx => ButtonLBPressed();
           // _controller.Player.ButtonLB.canceled -= ctx => ButtonLBReleased();
            _controller.Player.ItemUse.started -= ctx => ItemButtonPressed();
            _controller.Player.ItemUse.canceled -= ctx => ItemButonReleased();
            _controller.Player.ItemChangeRight.performed -= ctx => ItemChangeRight();
            _controller.Player.ItemChangeLeft.performed -= ctx => ItemChangeLeft();
            //_controller.Player.Quit.performed -= ctx => QuitKeyPressed();
            _controller.Player.ButtonStart.started -= ctx => ButtonStartPressed();

            CraftingSystem.CraftingSystem.OnMenuClose -= EnablePlayerInput;
            MainMenu.OnMenuClose -= EnablePlayerInput;
            TentMenu.OnTentMenuClose -= EnablePlayerInput;
            EventManager.PauseGame -= PauseGame;
            EventManager.UnpauseGame -= UnpauseGame;
            EventManager.PlayerGotBoots -= PlayerGotBoots;
            Door.EnablePlayerController -= IsPlayerInputEnabled;
            Trap.EnablePlayerController -= IsPlayerInputEnabled;
            MainMenu.OnMenuAction += IsPlayerInputEnabled;
        }


        //Used by the Input Proxy FSM of PlayMaker
        public Vector2 GetMovementInfo()
        {
            //print(movement);
            return movement;
        }

        private void ButtonAPressed()
        {
            Collider2D interactableObject = transform.parent.GetComponent<InteractableElements>().GetInteractableObject();
            if (interactableObject != null)
            {
                interactableObject.GetComponent<IInteractable>().Interact();
                EnableInput(false);
                return;
            }
            if (action1 == null) return;
            //action1.Execute(true);
            jumpButtonState = true;
            pMovement.SetJumpButtonState(true);
            SetJumpButtonStateOnMovement(jumpButtonState);
        }
        private void ButtonAReleased()
        {
            if (action1 == null) return;
            //action1.Execute(false);
            jumpButtonState = false;
            pMovement.SetJumpButtonState(false);
            SetJumpButtonStateOnMovement(jumpButtonState);
        }
        private void SetJumpButtonStateOnMovement(bool jumpState)
        {
            if (!!this.jumpButtonState && jumpState) pMovement.SetJumpButtonPress();
        }

        //Used by the Input Proxy FSM of PlayMaker
        public bool GetJumpButtonState()
        {
            return jumpButtonState;
        }



        private void ButtonXPressed()
        {
            if (buttonX == null) return;
            //print("Sending NORMALATTACK event to " + currentPMState.FsmName);
            currentPMState.SendEvent("NORMALATTACK");
            //buttonX.Execute(true);
        }
        private void ButtonXReleased()
        {
            if (buttonX == null) return;
            //buttonX.Execute(false);
        }



        private void ButtonBPressed()
        {
            if (buttonB == null) return;
            //print("Sending EVADE event to " + currentPMState.FsmName);
            currentPMState.SendEvent("EVASION");
            //buttonB.Execute(true);
        }

        private void ButtonBReleased()
        {
            if (buttonB == null) return;
            //buttonB.Execute(false);
        }

        private void ButtonYPressed()
        {
            if (buttonY == null) return;
            //buttonY.Execute(true);
            toolButtonState = true;
            //pMovement.ToolButtonPressedStatus(true);
            //currentPMState.SendEvent("TOOLACTION");
        }
        private void ButtonYReleased()
        {
            if (buttonY == null) return;
            //buttonY.Execute(false);
            toolButtonState = false;
            //pMovement.ToolButtonPressedStatus(false);
        }

        private void ButtonRBPressed()
        {
            if (buttonRB == null) return;
            //buttonRB.Execute(true);
            guardButtonState = true;
        }

        private void ButtonRBReleased()
        {
            if (buttonRB == null) return;
            //buttonRB.Execute(false);
            guardButtonState = false;
        }
        //Used by the Input Proxy FSM of PlayMaker
        public bool GetGuardButtonState()
        {
            return guardButtonState;
        }

        //Used by the Input Proxy FSM of PlayMaker
        public bool GetToolButtonState()
        {
            return toolButtonState;
        }

        private void ItemButonReleased()
        {

        }

        private void ItemButtonPressed()
        {
            print("Wants to use an item");
            currentPMState.SendEvent("USEITEM");
        }

        private void ItemChangeLeft()
        {
            print("Item Pressed Left");
            equipment.ChangeItem(false);
        }

        private void ItemChangeRight()
        {
            print("Item Pressed Right");
            equipment.ChangeItem(true);
        }

        private void ButtonStartPressed()
        {
            if (OnPlayerStartButtonPressed != null)
            {
                print("Player will open menu");
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
            _controller.UI.Enable();
        }
        void UnpauseGame()
        {
            print(gameObject.name + "  game was unpaused");
            IsPlayerInputEnabled(true);
            _controller.UI.Disable();
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
            this.interactableCollider = interactableCollider;
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
                print("Disabling input handler");
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
