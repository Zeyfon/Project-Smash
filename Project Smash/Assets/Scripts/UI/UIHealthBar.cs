using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHealthBar : MonoBehaviour
{

    private void OnEnable()
    {
        EventManager.PlayerIsDamaged += UpdateHealthScale;
    }

    private void OnDisable()
    {
        EventManager.PlayerIsDamaged -= UpdateHealthScale;
    }


    private void UpdateHealthScale(float healthScale)
    {
        transform.localScale = new Vector2(healthScale, transform.localScale.y);   
    }

}
