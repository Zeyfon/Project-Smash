using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Control;

public class EvadeCommand : ActionCommand, ICommand
{
    // Start is called before the first frame update
    bool isButtonPressed = false;
    void ICommand.Execute(PlayerController playerController)
    {
        isButtonPressed = !isButtonPressed;
        if (isButtonPressed) Evade(playerController);
    }

    void Evade(PlayerController playerController)
    {
        playerController.EvadeButtonPressed();
    }
}
