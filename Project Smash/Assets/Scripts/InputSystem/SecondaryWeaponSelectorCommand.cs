using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Control;

namespace PSmash.InputSystem
{
    public class SecondaryWeaponSelectorCommand : ActionCommand, ICommand
    {

        void ICommand.Execute(PlayerController playerController)
        {
            weaponSelector(playerController);
        }

        void weaponSelector(PlayerController playerController)
        {
            playerController.SecondaryWeaponSelectorPressed();
        }
    }
}

