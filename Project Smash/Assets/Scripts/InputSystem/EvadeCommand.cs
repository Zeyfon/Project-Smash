using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Control;

namespace PSmash.InputSystem
{
    public class EvadeCommand : ActionCommand, ICommand
    {

        void ICommand.Execute(PlayerController playerController)
        {
            Evade(playerController);
        }

        void Evade(PlayerController playerController)
        {
            playerController.EvadeButtonPressed();
        }
    }
}

