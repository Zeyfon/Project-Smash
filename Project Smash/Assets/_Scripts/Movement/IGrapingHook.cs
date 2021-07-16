using PSmash.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrapingHook
{
    //bool isEnemy();
    void Hooked();

    void Pulled();
}
