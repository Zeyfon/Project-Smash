using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Menus;

namespace PSmash.Attributes
{
    public class TimeManager : MonoBehaviour
    {
        [SerializeField] float slowDownFactor = 0.02f;

        private void EnableTimeScale(bool isEnabled)
        {
            if (isEnabled)
                Time.timeScale = 1;
            else
                Time.timeScale = 0;
        }

        public void SlowTime()
        {
            print("Slowing Time");
            Time.timeScale = slowDownFactor;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }

        public void SpeedUpTime()
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
}

