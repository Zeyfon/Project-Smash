using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using PSmash.Items;
using UnityEngine.UI;

namespace PSmash.UI
{
    public class ActionItemSlotUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI text = null;
        [SerializeField] Image image = null;

        public void UpdateItemInfo(ActionableItem actionableItem)
        {
            //print(actionableItem.name);
            //print(actionableItem.GetNumber());
            image.sprite = actionableItem.GetSprite();
            if (text == null)
                return;
            text.enabled = true;
            text.text = actionableItem.GetNumber().ToString();
        }
    }

}
