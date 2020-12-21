using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PSmash.Combat;

namespace PSmash.UI
{
    public class Items : MonoBehaviour
    {

        [SerializeField] Text text = null;
        

        // Start is called before the first frame update
        void Start()
        {
            text.text = FindObjectOfType<PlayerFighter>().currentItemQuantity.ToString();
        }

        private void OnEnable()
        {
            FindObjectOfType<PlayerFighter>().onItemThrown += ItemUsed;
        }

        private void OnDisable()
        {
            //FindObjectOfType<PlayerFighter>().onItemThrown -= ItemUsed;
        }

        void ItemUsed(int currentQuantity)
        {
            text.text = currentQuantity.ToString();
        }
    }
}

