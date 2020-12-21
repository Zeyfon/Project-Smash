using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace PSmash.Temporal
{
    public class GameManager : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Level Checked");
            AnalyticsEvent.LevelComplete(1);
        }
    }
}

