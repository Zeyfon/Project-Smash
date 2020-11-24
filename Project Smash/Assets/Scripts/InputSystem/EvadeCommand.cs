using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Control;

namespace PSmash.InputSystem
{
    public class EvadeCommand : ActionCommand, ICommand
    {
        PlayerController playerController;

        void Start()
        {
            playerController = transform.parent.GetComponent<PlayerController>();
        }
        void ICommand.Execute(bool isButtonPressed)
        {
            if (isButtonPressed) Evade();
        }

        void Evade()
        {
            playerController.EvadeButton();
        }
    }
}

