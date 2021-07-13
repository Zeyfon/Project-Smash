using PSmash.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrapingHook
{

    void Hooked(Transform attackerTransform);

    void Pulled();
}
