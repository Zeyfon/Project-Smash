using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class LevelProgressionAnalytics : MonoBehaviour
{

    [SerializeField] int level;
    public static int currentSection = 0;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GetComponent<Collider2D>().enabled = false;
            AnalyticsEvent.LevelComplete(level);

            currentSection = level;
        }
    }
}
