using UnityEngine;
using PSmash.Control;
using UnityEngine.InputSystem.Composites;

namespace PSmash.InputSystem
{
    public class JumpCommand : ActionCommand, ICommand
    {
        PlayerController playerController;

        void Start()
        {
            playerController = transform.parent.GetComponent<PlayerController>();
        }
        void ICommand.Execute(bool isButtonPressed)
        {
            if (isButtonPressed) Jump();
        }

        void Jump()
        {
            playerController.JumpButtonPressed();
        }

    }
}

