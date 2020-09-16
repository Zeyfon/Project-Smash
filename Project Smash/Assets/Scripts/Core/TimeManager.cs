using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Core
{
    public class TimeManager : MonoBehaviour
    {
        [SerializeField] float slowDownFactor = 0.02f;

        public void SlowTime()
        {
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

