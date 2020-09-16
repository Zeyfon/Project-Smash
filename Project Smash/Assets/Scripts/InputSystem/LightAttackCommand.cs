using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Control;

namespace PSmash.InputSystem
{
    public class LightAttackCommand : ActionCommand, ICommandV2
    {
        PlayerControllerV2 playerController;

        void Start()
        {
            playerController = transform.parent.GetComponent<PlayerControllerV2>();
        }
        void ICommandV2.Execute(bool isButtonPressed)
        {
            if (isButtonPressed) Attack();
        }

        void Attack()
        {
            playerController.MainAttackButton(true);
        }
    }
}

