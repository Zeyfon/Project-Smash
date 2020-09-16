using PSmash.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PSmash.InputSystem
{
    public class InteractCommand : ActionCommand, ICommand
    {
        bool isButtonPressed = false;
        // Start is called before the first frame update
        void ICommand.Execute(PlayerController playerController)
        {
            isButtonPressed = !isButtonPressed;
            Interact(isButtonPressed, playerController);
        }
        void Interact(bool isButtonPessed, PlayerController playerController)
        {
            playerController.InteractButtonPressed(isButtonPressed);

            if (isButtonPressed) print("Interaction Button is pressed");
            if (!isButtonPressed) print("Interaction Button is released");
        }

    }
}

