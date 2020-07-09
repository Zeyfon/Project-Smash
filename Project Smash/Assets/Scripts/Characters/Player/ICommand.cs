using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Control;

public interface ICommand
{
    void Execute(PlayerController playerController);
}
