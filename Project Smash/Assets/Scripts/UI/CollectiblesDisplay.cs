using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollectiblesDisplay : MonoBehaviour
{
    Text text;

    int counter = 0;
    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<Text>();
        text.text = counter.ToString();
    }

    private void OnEnable()
    {
        Star.StartObtained += CollectibleObtained;
    }

    private void CollectibleObtained()
    {
        counter++;
        text.text = counter.ToString();
    }
}
