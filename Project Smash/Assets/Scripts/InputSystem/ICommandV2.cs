using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Control;

namespace PSmash.InputSystem
{
    public interface ICommandV2
    {
        void Execute(bool isButtonPressed);
    }
}

