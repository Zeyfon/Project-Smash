using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Control;

namespace PSmash.InputSystem
{
    public class SecondaryWeaponCommand : ActionCommand, ICommand
    {
        void ICommand.Execute(PlayerController playerController)
        {
            SubAttack(playerController);
        }

        void SubAttack(PlayerController playerController)
        {
            playerController.SecondaryWeaponButtonPressed();
        }

    }
}

