using PSmash.Combat;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;
using UnityEngine.UI;
using TMPro;

namespace PSmash.UI
{
    public class UIHealth : MonoBehaviour
    {
        [SerializeField] Transform healthBar = null;
        [SerializeField] TextMeshProUGUI currentHealthText = null;
        [SerializeField] TextMeshProUGUI maxHealthText = null;

        PlayerHealth health;
        

        private void Awake()
        {
            health = FindObjectOfType<PlayerHealth>();
        }


        private void Update()
        {
            //text.text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
            healthBar.localScale = new Vector2(health.GetHealth() / health.GetMaxHealthPoints(), transform.localScale.y);
            currentHealthText.text = health.GetHealth().ToString();
            maxHealthText.text = health.GetMaxHealthPoints().ToString();

        }
        private void OnEnable()
        {
            health.UpdateUIHealth += UpdateHealthScale;
        }

        private void OnDisable()
        {
            health.UpdateUIHealth -= UpdateHealthScale;
        }

        private void UpdateHealthScale(float health, float initialHealth)
        {
            healthBar.localScale = new Vector2(health / initialHealth, transform.localScale.y);
            currentHealthText.text = health.ToString();
            maxHealthText.text = initialHealth.ToString();
        }

    }
}

