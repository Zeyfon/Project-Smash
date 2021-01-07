using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Control;

namespace PSmash.InputSystem
{
    public class LightAttackCommand : ActionCommand, ICommand
    {
        PlayerController playerController;

        void Start()
        {
            playerController = transform.parent.GetComponent<PlayerController>();
        }
        void ICommand.Execute(bool isButtonPressed)
        {
            if (isButtonPressed) Attack();
        }

        void Attack()
        {
            //playerController.MainAttackButton(true);
        }
    }
}

