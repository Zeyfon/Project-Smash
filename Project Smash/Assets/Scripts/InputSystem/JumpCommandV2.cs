using UnityEngine;
using PSmash.Control;
using UnityEngine.InputSystem.Composites;

namespace PSmash.InputSystem
{
    public class JumpCommandV2 : ActionCommand, ICommandV2
    {
        PlayerControllerV2 playerController;

        void Start()
        {
            playerController = transform.parent.GetComponent<PlayerControllerV2>();
        }
        void ICommandV2.Execute(bool isButtonPressed)
        {
            if (isButtonPressed) Jump();
        }

        void Jump()
        {
            playerController.JumpButtonPressed();
        }

    }
}

