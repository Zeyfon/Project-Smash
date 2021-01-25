using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PSmash.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text;

        private void Update()
        {
            transform.rotation = Quaternion.identity;
        }
        public void SetDamagaeValue(float amount)
        {
            text.text = String.Format("{0:0}", amount);
        }
        void DestroyText()
        {
            Destroy(gameObject);
        }
    }
}

