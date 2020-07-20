using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SecondaryWeaponsUI : MonoBehaviour
{
    private void OnEnable()
    {
        //EventManager.SubWeaponchangeDone += DisplayThisWeaponImage;
    }
    private void OnDisable()
    {
        //EventManager.SubWeaponchangeDone -= DisplayThisWeaponImage;

    }

    private void DisplayThisWeaponImage(Sprite weaponEquipped)
    {
        GetComponent<Image>().sprite = weaponEquipped;
    }
}
