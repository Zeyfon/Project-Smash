﻿using UnityEngine;
using PSmash.Control;
using UnityEngine.InputSystem.Composites;

public class ParryCommand : ActionCommand, ICommand
{
    void ICommand.Execute(PlayerController playerController)
    {
        Parry(playerController);
    }

    void Parry(PlayerController playerController)
    {
        playerController.ParryButtonPressed();
    }

}
