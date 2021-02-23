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
        [SerializeField] float timeToDecrease = 1;
        [SerializeField] float fillSpeed = 5;
        [SerializeField] Image healthBar = null;
        [SerializeField] Image damageBar = null;
        [SerializeField] TextMeshProUGUI currentHealthText = null;
        [SerializeField] TextMeshProUGUI maxHealthText = null;

        PlayerHealth health;
        float frontalBarHealthValue = 0;
        float damageTimer = 0;

        private void Awake()
        {
            health = FindObjectOfType<PlayerHealth>();
        }


        private void Update()
        {
            damageTimer -= Time.deltaTime;
            if (damageTimer < 0)
            {
                if (healthBar.fillAmount < damageBar.fillAmount)
                {
                    damageBar.fillAmount -= Time.deltaTime / fillSpeed;
                }
            }

            //text.text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
            //if(healthBar.localScale <)
            //healthBar.localScale = new Vector2(health.GetHealth() / health.GetMaxHealthPoints(), transform.localScale.y);
            //currentHealthText.text = health.GetHealth().ToString();
            //maxHealthText.text = health.GetMaxHealthPoints().ToString();
        }

        private void OnEnable()
        {
            health.onDamaged += OnDamaged;
            health.onHealed += OnHealed;

        }

        private void OnHealed()
        {
            print("UI Health Healed");
            healthBar.fillAmount = health.GetHealth() / health.GetMaxHealthPoints();
            damageBar.fillAmount = healthBar.fillAmount;
            UpdateText();
        }

        private void OnDamaged()
        {
            print("UI Health Damaged");
            damageTimer = timeToDecrease;
            healthBar.fillAmount =  health.GetHealth() / health.GetMaxHealthPoints();
            UpdateText();
        }

        private void UpdateText()
        {
            currentHealthText.text = health.GetHealth().ToString();
            maxHealthText.text = health.GetMaxHealthPoints().ToString();
        }

    }
}

