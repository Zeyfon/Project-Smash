using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using PSmash.Attributes;
using PSmash.Items.Doors;

namespace PSmash.UI
{
    public class InGameKeyDoorUI : MonoBehaviour
    {
        [SerializeField] Text currentKeysText = null;
        [SerializeField] Text keysRequiredText = null;


        private void OnEnable()
        {
            DoorKey.OnDoorOpening += DisableInGameUI;
        }

        private void OnDisable()
        {
            DoorKey.OnDoorOpening -= DisableInGameUI;
        }

        //private void OnDisable
        //Initially Set the values shown in tye UI
        public void InitializeUI(int currentKeys, int keysRequired)
        {
            currentKeysText.text = currentKeys.ToString();
            keysRequiredText.text = keysRequired.ToString();
        }

        //Update current key quantity in the UI
        public void UpdateUI(int currentKeys)
        {
            currentKeysText.text = currentKeys.ToString();
        }

        //Proxy to be used by the DoorKey Class to start fading Out the UI
        public void DisableInGameUI(InteractionList myValue)
        {
            if (myValue != transform.parent.GetComponent<DoorKey>().doorID) return;
            StartCoroutine(FadeOut());
        }

        //Fading Out UI Method
        IEnumerator FadeOut()
        {
            print("Disabling UI");
            CanvasGroup canvasgroup = GetComponent<CanvasGroup>();
            float alpha = canvasgroup.alpha;
            while (alpha > 0)
            {
                alpha -= Time.deltaTime/2;
                if (alpha < 0) alpha = 0;
                canvasgroup.alpha = alpha;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}

