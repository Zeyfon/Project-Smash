using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PSmash.Menus
{
    public class Status : MonoBehaviour
    {
        [SerializeField] Text maxStarsText = null;
        [SerializeField] Text currentStarsText = null;

        int currentStarsQuantity = 0;

        void Start()
        {
            maxStarsText.text = PSmash.Items.Star.starsQuantity.ToString();
            if (gameObject.activeInHierarchy) gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            currentStarsText.text = currentStarsQuantity.ToString();
        }
        public void StarCollected()
        {
            currentStarsQuantity += 1;
        }
    }
}

