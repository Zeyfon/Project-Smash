using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Attributes
{

    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Transform bar = null;

        EnemyHealth enemyHealth;
        private void Awake()
        {
            enemyHealth = transform.parent.transform.GetComponentInChildren<EnemyHealth>();
            if(enemyHealth == null)
                enemyHealth = GetComponentInParent<EnemyHealth>();

        }
        private void Update()
        {
            transform.rotation = Quaternion.identity;
            bar.localScale = new Vector2(enemyHealth.GetHealthValue() / enemyHealth.GetMaxHealth(), transform.localScale.y) ;
        }

        //UNITY EVENT
        public void DisableGameObject()
        {
            gameObject.SetActive(false);
        }
    }
}

