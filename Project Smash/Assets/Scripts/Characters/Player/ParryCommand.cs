using UnityEngine;
using PSmash.Control;
using UnityEngine.InputSystem.Composites;

public class ParryCommand : ActionCommand, ICommand
{
    bool isButtonPressed = false;
    void ICommand.Execute(PlayerController playerController)
    {
        isButtonPressed = !isButtonPressed;
        if(isButtonPressed) Parry(playerController);
    }

    void Parry(PlayerController playerController)
    {
        playerController.ParryButtonPressed();
    }

}
