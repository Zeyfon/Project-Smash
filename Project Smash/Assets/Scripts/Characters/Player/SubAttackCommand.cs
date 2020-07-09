using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Control;

public class SubAttackCommand : ActionCommand, ICommand
{
    bool isButtonPressed = false;
    void ICommand.Execute(PlayerController playerController)
    {
        isButtonPressed = !isButtonPressed;
        if (isButtonPressed) SubAttack(playerController);
    }

    void SubAttack(PlayerController playerController)
    {
        playerController.SubAttackButtonPressed();
    }

}
