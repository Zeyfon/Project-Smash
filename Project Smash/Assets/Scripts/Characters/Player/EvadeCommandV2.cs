using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Control;

public class EvadeCommandV2 : ActionCommand, ICommandV2
{
    PlayerControllerV2 playerController;

    void Start()
    {
        playerController = transform.parent.GetComponent<PlayerControllerV2>();
    }
    void ICommandV2.Execute(bool isButtonPressed)
    {
        if(isButtonPressed) Evade();
    }

    void Evade()
    {
        playerController.EvadeButton();
    }
}
