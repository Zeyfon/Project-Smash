using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace PSmash.LevelUpSystem
{
    public class UICraftingMaterial : MonoBehaviour
    {
        public CraftingMaterialsList material;
        [SerializeField] TextMeshProUGUI requiredQuantityText = null;
        [SerializeField] TextMeshProUGUI textName = null;
        [SerializeField] Image image = null;

        /// <summary>
        /// Here only updates the required material current quantity
        /// </summary>

        private void Start()
        {
            if (image.sprite == null)
            {
                textName.text = material.ToString();
            }
        }

        public void UpdateValue(int value)
        {
            requiredQuantityText.text = value.ToString();
        }
    }
}

