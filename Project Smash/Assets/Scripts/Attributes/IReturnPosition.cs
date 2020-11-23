using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{
    public interface IReturnPosition
    {
        void SetNewPosition(Vector3 lastPosition);
    }

}
