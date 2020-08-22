using PSmash.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHealthBar : MonoBehaviour
{
    PlayerHealth playerHealth;

    private void Awake()
    {
        playerHealth = FindObjectOfType<PlayerHealth>();
    }
    private void OnEnable()
    {
        playerHealth.OnPlayerDamage += UpdateHealthScale;
    }

    private void OnDisable()
    {
        playerHealth.OnPlayerDamage -= UpdateHealthScale;
    }

    private void UpdateHealthScale(float healthScale)
    {
        transform.localScale = new Vector2(healthScale, transform.localScale.y);   
    }

}
