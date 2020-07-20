using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Control;

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
