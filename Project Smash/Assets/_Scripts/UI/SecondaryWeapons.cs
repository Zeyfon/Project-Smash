using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PSmash.UI
{
    public class SecondaryWeapons : MonoBehaviour
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
}

