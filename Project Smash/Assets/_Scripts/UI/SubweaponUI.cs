using PSmash.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameDevTV.Saving;

namespace PSmash.UI
{
    public class SubweaponUI : MonoBehaviour, ISaveable
    {
        [SerializeField] Image image = null;

        private void Awake()
        {
            image.enabled = false;
        }

        private void OnEnable()
        {
            Mace.onObjectTaken += ShowSubweapon;
        }
        private void OnDisable()
        {
            Mace.onObjectTaken -= ShowSubweapon;
        }

        private void ShowSubweapon(bool isEnabled)
        {
            if (!isEnabled)
            {
                image.enabled = true;
            }
        }

        public object CaptureState()
        {
            return image.enabled;
        }

        public void RestoreState(object state)
        {
            bool isEnabled = (bool)state;
            image.enabled = isEnabled;
        }
    }
}

