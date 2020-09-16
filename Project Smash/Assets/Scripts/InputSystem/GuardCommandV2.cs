using UnityEngine;
using PSmash.Control;
using UnityEngine.InputSystem.Composites;

namespace PSmash.InputSystem
{
    public class GuardCommandV2 : ActionCommand, ICommandV2
    {
        PlayerControllerV2 playerController;

        void Start()
        {
            playerController = transform.parent.GetComponent<PlayerControllerV2>();
        }
        void ICommandV2.Execute(bool isButtonPressed)
        {
            GuardAction(isButtonPressed);
        }
        void GuardAction(bool isButtonPressed)
        {
            playerController.GuardButton(isButtonPressed);
        }
    }
}

