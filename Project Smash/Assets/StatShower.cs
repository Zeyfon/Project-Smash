using PSmash.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PSmash.Stats;

public class StatShower : MonoBehaviour
{
    enum Stat
    {
        Health,
        Attack,
        Defense
    }

    [SerializeField] Stat myStat;
    [SerializeField] TextMeshProUGUI text = null;

    void OnEnable()
    {
        UpdateStat(myStat);
    }
    void UpdateStat(Stat myStat)
    {
        if(myStat == Stat.Health)
           text.text = FindObjectOfType<PlayerHealth>().GetHealth().ToString();
        else if(myStat == Stat.Attack)
        {
            text.text = FindObjectOfType<PlayerHealth>().GetComponent<BaseStats>().GetStat(StatsList.Attack).ToString();
        }
        else if (myStat == Stat.Defense)
        {
            text.text = FindObjectOfType<PlayerHealth>().GetComponent<BaseStats>().GetStat(StatsList.Defense).ToString();
        }
    }
}
