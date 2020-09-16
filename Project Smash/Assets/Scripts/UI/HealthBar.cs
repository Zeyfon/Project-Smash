using PSmash.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Resources;

namespace PSmash.UI
{
    public class HealthBar : MonoBehaviour
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

        private void UpdateHealthScale(float health, float initialHealth)
        {
            transform.localScale = new Vector2(health / initialHealth, transform.localScale.y);
        }

    }
}

