using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PSmash.InputSystem;
using PSmash.Resources;

public class InputButton : MonoBehaviour
{
    [SerializeField] Image image = null;
    [SerializeField] ActionList myAction;
    [SerializeField] List<Sprite> sprites = new List<Sprite>();
    public ButtonList myButton;

    Transform inputTransform;
    InputHandlerV2 inputHanlder;

    bool isInitiallySet = false;
    public void GetInitialButton()
    {
        Debug.Log(this.gameObject + "  " + myButton);

        if (!inputHanlder) inputHanlder = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).GetComponent<InputHandlerV2>();
        List<ICommandV2> commands = inputHanlder.commandList;
        switch (myAction)
        {
            case ActionList.Jump:
                foreach (ICommandV2 com in commands)
                {
                    if (com is JumpCommandV2)
                    {
                        SetButton((com as JumpCommandV2).myButton);
                    }
                }
                break;
            case ActionList.Attack:
                foreach(ICommandV2 com in commands)
                {
                    //if(com is AttackCommand)
                    //{
                    //    SetButton((com as AttackCommand).myButton);
                    //}
                }
                break;
            case ActionList.Interact:
                foreach (ICommandV2 com in commands)
                {
                    //if (com is InteractCommand)
                    //{
                    //    SetButton((com as InteractCommand).myButton);

                    //}
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
        //inputHanlder.SwitchButtonFromMenu(myButton, myAction);

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