using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Resources
{

    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Transform bar = null;

        EnemyHealth enemyHealth;
        private void Awake()
        {
            enemyHealth = transform.parent.transform.GetComponentInChildren<EnemyHealth>();
        }
        private void Update()
        {
            bar.localScale = new Vector2(enemyHealth.GetHealthValue() / enemyHealth.GetInitialHealthValue(), transform.localScale.y) ;
        }
    }
}

