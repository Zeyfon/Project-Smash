using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;


public class InputButton : MonoBehaviour
{
    [SerializeField] Image image = null;
    [SerializeField] ActionList myAction;
    [SerializeField] List<Sprite> sprites = new List<Sprite>();
    public ButtonList myButton;

    Transform inputTransform;
    InputHandler inputHanlder;

    bool isInitiallySet = false;
    public void GetInitialButton()
    {
        Debug.Log(this.gameObject + "  " + myButton);

        if (!inputHanlder) inputHanlder = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetComponent<InputHandler>();
        List<ICommand> commands = inputHanlder.commandList;
        switch (myAction)
        {
            case ActionList.Jump:
                foreach (ICommand com in commands)
                {
                    if (com is JumpCommand)
                    {
                        SetButton((com as JumpCommand).myButton);
                    }
                }
                break;
            case ActionList.Attack:
                foreach(ICommand com in commands)
                {
                    if(com is AttackCommand)
                    {
                        SetButton((com as AttackCommand).myButton);
                    }
                }
                break;
            case ActionList.Interact:
                foreach (ICommand com in commands)
                {
                    if (com is InteractCommand)
                    {
                        SetButton((com as InteractCommand).myButton);

                    }
                }
                break;
            default:
                break;
        }
        transform.parent.SendMessage("InitialChildrenSet");
    }

   
    public void SetButton(ButtonList button)
    {
        //Debug.Log(this.gameObject + "  " + myButton);
        //print(myButton);
        myButton = button;
        ChangeButtonImage();
        if (!isInitiallySet)
        {
            isInitiallySet = true;
            return;
        }
        inputHanlder.SwitchButtonFromMenu(myButton, myAction);

    }

    private void ChangeButtonImage()
    {
        switch (myButton)
        {
            case ButtonList.buttonA:
                image.sprite = sprites[(int)ButtonList.buttonA];
                break;
            case ButtonList.buttonB:
                image.sprite = sprites[(int)ButtonList.buttonB];
                break;
            case ButtonList.buttonX:
                image.sprite = sprites[(int)ButtonList.buttonX];
                break;
            case ButtonList.buttonY:
                image.sprite = sprites[(int)ButtonList.buttonY];
                break;
            case ButtonList.noButton:
                image.sprite = sprites[(int)ButtonList.noButton];
                break;
        }
    }
}