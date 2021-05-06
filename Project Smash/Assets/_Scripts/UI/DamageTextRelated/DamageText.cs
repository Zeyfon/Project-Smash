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
        public void Setup(float amount, Color color, float sizeFactor)
        {
            text.fontSize *= sizeFactor;
            text.color = color;
            text.text = String.Format("{0:0}", amount);
        }
        void DestroyText()
        {
            Destroy(gameObject);
        }
    }
}

