using UnityEngine;
using PSmash.Control;
using UnityEngine.InputSystem.Composites;

namespace PSmash.InputSystem
{
    public class GuardCommand : ActionCommand, ICommand
    {
        PlayerController playerController;

        void Start()
        {
            playerController = transform.parent.GetComponent<PlayerController>();
        }
        void ICommand.Execute(bool isButtonPressed)
        {
            GuardAction(isButtonPressed);
        }
        void GuardAction(bool isButtonPressed)
        {
            //playerController.GuardButton(isButtonPressed);
        }
    }
}

