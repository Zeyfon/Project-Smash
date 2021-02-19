using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PSmash.Items;
using UnityEngine.UI;

namespace PSmash.UI
{
    public class UIItem : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text = null;
        [SerializeField] Image image = null;

        public void UpdateItemInfo(Item item, int value)
        {
            image.sprite = item.sprite;
            text.enabled = true;
            text.text = value.ToString();
        }
        public void UpdateItemInfo(Item item)
        {
            image.sprite = item.sprite;
            //text.enabled = false;
        }
    }

}
