using UnityEngine;
using PSmash.Control;
using UnityEngine.InputSystem.Composites;

public class JumpCommand : ActionCommand, ICommand
{
    bool isButtonPressed = false;
    void ICommand.Execute(PlayerController playerController)
    {
        isButtonPressed = !isButtonPressed;
        if(isButtonPressed) Jump(playerController);
    }

    void Jump(PlayerController playerController)
    {
        playerController.JumpButtonPressed();
    }

}
