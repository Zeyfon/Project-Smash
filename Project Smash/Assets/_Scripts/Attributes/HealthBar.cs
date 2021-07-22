using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{

    public class HealthBar : MonoBehaviour
    {
        [SerializeField] bool isBarFixedLength = false;
        [SerializeField] Transform bar = null;
        [SerializeField] Transform background = null;
        [SerializeField] float healthValueForFixedLength = 300;
        [Tooltip("The value the bar will have when fixed to healthValueForFiexLength")]
        [SerializeField] float xScale = 1.5f;

        EnemyHealth enemyHealth;
        private void Awake()
        {
            enemyHealth = GetComponentInParent<EnemyHealth>();     
        }

        private void Start()
        {
            float xScale;

            if (!isBarFixedLength)
            {
                xScale = enemyHealth.GetMaxHealth() * this.xScale / healthValueForFixedLength;
            }
            else
            {
                xScale = this.xScale;
            }
            xScale += 0.05f;
            background.localScale = new Vector2(xScale, background.localScale.y);
        }
        private void Update()
        {
            float xLocalScale;

            transform.rotation = Quaternion.identity;
            if (!isBarFixedLength)
            {
                xLocalScale = (enemyHealth.GetHealthValue() * xScale / healthValueForFixedLength);
            }
            else
            {
                xLocalScale = (enemyHealth.GetHealthValue() / enemyHealth.GetMaxHealth())*xScale;
            }
            bar.localScale = new Vector2(xLocalScale, bar.localScale.y);

        }

        //UNITY EVENT
        public void DisableGameObject()
        {
            gameObject.SetActive(false);
        }
    }
}

