using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

namespace PSmash.Menus
{
    public class WeaponsMenuController : MonoBehaviour
    {
        [SerializeField] float scaleFactor = 1.2f;
        [SerializeField] Transform weaponRight = null;
        [SerializeField] Transform weaponLeft = null;
        [SerializeField] Transform weaponUp = null;
        [SerializeField] Transform weaponDown = null;
        _Controller _controller;

        private void Awake()
        {
            _controller = new _Controller();
        }

        private void OnEnable()
        {
            _controller.WeaponSelection.Enable();
            _controller.WeaponSelection.DPadDown.performed += ctx => DPadDownPressed();
            _controller.WeaponSelection.DPadUp.performed += ctx => DPadUpPressed();
            _controller.WeaponSelection.DPadLeft.performed += ctx => DPadLeftPressed();
            _controller.WeaponSelection.DPadRight.performed += ctx => DPadRightPressed();
        }

        private void Ondisable()
        {
            _controller.WeaponSelection.Disable();
            _controller.WeaponSelection.DPadDown.performed -= ctx => DPadDownPressed();
            _controller.WeaponSelection.DPadUp.performed -= ctx => DPadUpPressed();
            _controller.WeaponSelection.DPadLeft.performed -= ctx => DPadLeftPressed();
            _controller.WeaponSelection.DPadRight.performed -= ctx => DPadRightPressed();
        }

        private void DPadRightPressed()
        {
            Debug.Log("DPad Right Pressed");
            weaponRight.localScale = new Vector3(scaleFactor, scaleFactor, 1);
            weaponLeft.localScale = new Vector3(1, 1, 1);
            weaponUp.localScale = new Vector3(1, 1, 1);
            weaponDown.localScale = new Vector3(1, 1, 1);
        }

        private void DPadLeftPressed()
        {
            Debug.Log("DPad Left Pressed");
            weaponRight.localScale = new Vector3(1, 1, 1);
            weaponLeft.localScale = new Vector3(scaleFactor, scaleFactor, 1);
            weaponUp.localScale = new Vector3(1, 1, 1);
            weaponDown.localScale = new Vector3(1, 1, 1);
        }

        private void DPadUpPressed()
        {
            weaponRight.localScale = new Vector3(1, 1, 1);
            weaponLeft.localScale = new Vector3(1, 1, 1);
            weaponUp.localScale = new Vector3(scaleFactor, scaleFactor, 1);
            weaponDown.localScale = new Vector3(1, 1, 1);
            Debug.Log("DPad Up Pressed");
        }

        private void DPadDownPressed()
        {
            weaponRight.localScale = new Vector3(1, 1, 1);
            weaponLeft.localScale = new Vector3(1, 1, 1);
            weaponUp.localScale = new Vector3(1, 1, 1);
            weaponDown.localScale = new Vector3(scaleFactor, scaleFactor, 1);
            Debug.Log("DPad Down Pressed");
        }
    }

}

