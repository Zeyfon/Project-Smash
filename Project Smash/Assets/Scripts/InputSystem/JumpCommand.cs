using UnityEngine;
using PSmash.Control;
using UnityEngine.InputSystem.Composites;

namespace PSmash.InputSystem
{
    public class JumpCommand : ActionCommand, ICommand
    {
        void ICommand.Execute(PlayerController playerController)
        {
            Jump(playerController);
        }

        void Jump(PlayerController playerController)
        {
            playerController.JumpButtonPressed();
        }

    }
}

