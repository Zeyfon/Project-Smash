using PSmash.Attributes;
using PSmash.Control;
using PSmash.Items.Doors;
using PSmash.Items.Traps;
using PSmash.Items;
using PSmash.Movement;
using System;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Saving;
using UnityEngine.InputSystem;
using HutongGames.PlayMaker;

namespace PSmash.InputSystem
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] PlayerMovement pMovement = null;
        [SerializeField] ItemHandler itemsHandler = null;

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
        //PlayerController playerController;
        _Controller _controller;

        Vector2 movement;
        float itemSelect;
        bool jumpButtonState = false;
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
            SetCommandList();
            //SetButtonsInControllerMenu();
        }

        //private void Update()
        //{
        //    ItemChange();
        //}

        private void ItemChange()
        {
            print("Button pressed");
            if (Mathf.Approximately(itemSelect, 1))
            {
                itemsHandler.ChangeItem(true);
            }
            else if (Mathf.Approximately(itemSelect, -1))
            {
                itemsHandler.ChangeItem(false);
            }
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

        public Vector2 GetMovementInfo()
        {
            //print(movement);
            return movement;
        }
        
        public bool GetJumpButtonState()
        {
            return jumpButtonState;
        }

        public bool GetGuardButtonState()
        {
            return guardButtonState;
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
            _controller.Player.ButtonLB.canceled += ctx => ButtonLBReleased();
            _controller.Player.ItemUse.started += ctx => ItemButtonPressed();
            _controller.Player.ItemUse.canceled += ctx => ItemButonReleased();
            _controller.Player.ItemChangeRight.performed += ctx => ItemChangeRight();
            _controller.Player.ItemChangeLeft.performed += ctx => ItemChangeLeft();


            _controller.Player.ButtonStart.started += ctx => ButtonStartPressed();
            //_controller.Player.Quit.performed += ctx => QuitKeyPressed();
            EventManager.PauseGame += PauseGame;
            EventManager.UnpauseGame += UnpauseGame;
            EventManager.PlayerGotBoots += PlayerGotBoots;
            EventManager.PlayerPerformUncontrolledAction += EnablePlayerController;
            Door.EnablePlayerController += EnablePlayerController;
            PlayerMovement.EnablePlayerController += EnablePlayerController;
            Trap.EnablePlayerController += EnablePlayerController;
            Menus.Menus.OnMenusClosed += EnablePlayerController;
        }

        private void ItemChangeLeft()
        {
            itemsHandler.ChangeItem(false);
        }

        private void ItemChangeRight()
        {
            itemsHandler.ChangeItem(true);
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
            _controller.Player.ButtonLB.canceled -= ctx => ButtonLBReleased();
            _controller.Player.ItemUse.started -= ctx => ItemButtonPressed();
            _controller.Player.ItemUse.canceled -= ctx => ItemButonReleased();

            //_controller.Player.Quit.performed -= ctx => QuitKeyPressed();
            _controller.Player.ButtonStart.started -= ctx => ButtonStartPressed();
            EventManager.PauseGame -= PauseGame;
            EventManager.UnpauseGame -= UnpauseGame;
            EventManager.PlayerGotBoots -= PlayerGotBoots;
            Door.EnablePlayerController -= EnablePlayerController;
            PlayerMovement.EnablePlayerController -= EnablePlayerController;
            Trap.EnablePlayerController -= EnablePlayerController;
            Menus.Menus.OnMenusClosed -= EnablePlayerController;
        }

        private void ItemButonReleased()
        {

        }

        private void ItemButtonPressed()
        {
            print("Wants to use an item");
            currentPMState.SendEvent("USEITEM");
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

        public Vector2 GetMovement()
        {
            return movement;
        }

        #region ControllerButtons
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

        private void ButtonAPressed()
        {
            if (interactableCollider != null)
            {
                //this.enabled = false;
                interactableCollider.GetComponent<IInteractable>().StartInteracting();
                return;
            }
            if (action1 == null) return;
            //action1.Execute(true);
            jumpButtonState = true;
            pMovement.SetJumpButtonState(true) ;
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

        private void ButtonYPressed()
        {
            if (buttonY == null) return;
            //buttonY.Execute(true);
            pMovement.ToolButtonPressedStatus(true);
            //currentPMState.SendEvent("TOOLACTION");
        }
        private void ButtonYReleased()
        {
            if (buttonY == null) return;
            //buttonY.Execute(false);
            pMovement.ToolButtonPressedStatus(false);

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

        private void ButtonLBPressed()
        {
            //print("Want to throw item");
            if (buttonLB == null) return;
            //buttonLB.Execute(playerController);
            pMovement.ThrowDaggerButtonJustPressed(true);
        }
        private void ButtonLBReleased()
        {
            //print("Want to throw item");
            if (buttonLB == null) return;
            //buttonLB.Execute(playerController);
            pMovement.ThrowDaggerButtonJustPressed(false);
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
            //timeManager.SlowTime();
            //weaponsMenu.SetActive(true);
        }
        private void LeftTriggerReleased()
        {
            Debug.Log("Left Trigger Released");
            //timeManager.SpeedUpTime();
            //weaponsMenu.SetActive(false);
        }

        private void ButtonStartPressed()
        {
            if (OnPlayerStartButtonPressed != null)
            {
                print("Player will open menu");
                //input.currentActionMap = _controller.UI;
                _controller.Player.Disable();
                OnPlayerStartButtonPressed();
                //_controller.UI.Enable();
                EnablePlayerController(false);
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
                //playerController.SetEnable(false);
                //if (pmPlayerController != null) pmPlayerController.enabled = false; 
                //print("InputHandler Disabled");
            }
            else
            {
                //playerController.SetEnable(true);
                if (pmPlayerController != null) pmPlayerController.enabled = true;
                if (!_controller.Player.enabled) 
                {
                    _controller.Player.Enable();
                    //Time.timeScale = 1;
                }
                //print("InputHandler Enabled");
            }
        }

        public void SetInteractableCollider(Collider2D interactableCollider)
        {
            this.interactableCollider = interactableCollider;
        }

        public void EnableInput(bool state)
        {
            if (state)
            {
                //print("Enabling input handler");
                _controller.Player.Enable();
            }
            else
            {
                //print("Disabling input handler");
                _controller.Player.Disable();
                movement = new Vector2(0, 0);
            }
            this.enabled = state;
        }

        #endregion

        //Here the buttons are set to their initial commands. In a next update the buttons will get 
        //the command to what they where attached in their previous playsesion


        #region Update Button Mapping
        //This list is needed to the Button Change Mechanic 
        void SetCommandList()
        {
            commandList.Add(action1);
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
