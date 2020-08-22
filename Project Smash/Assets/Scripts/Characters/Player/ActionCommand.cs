using PSmash.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionCommand : MonoBehaviour
{
   
    public ButtonList myButton;

    public void ChangeMyActionButton(ICommand com,ButtonList newButton)
    {
        myButton = newButton;
    }
}
