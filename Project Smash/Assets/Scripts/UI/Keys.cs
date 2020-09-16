using UnityEngine;
using UnityEngine.UI;
using PSmash.Resources;
using System.Collections;
using PSmash.Items.Doors;

namespace PSmash.UI
{
    public class Keys : MonoBehaviour
    {
        [SerializeField] Text currentKeysText;
        [SerializeField] float fadeOutTime = 2;

        InteractionList currentValue;

        CanvasGroup canvasGroup;
        int currentKeys = 0;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        private void Start()
        {
            currentKeysText.text = "0";
            canvasGroup.alpha = 0;
        }

        private void OnEnable()
        {
            Key.OnKeyTaken += KeyCollected;
            PSmash.Items.Doors.KeyDoor.OnDoorOpening += DisableKeysUI;
        }

        private void OnDisable()
        {
            Key.OnKeyTaken -= KeyCollected;
            PSmash.Items.Doors.KeyDoor.OnDoorOpening -= DisableKeysUI;
        }

        private void KeyCollected(InteractionList myValue)
        {
            currentKeys++;
            if (currentKeys > 0) canvasGroup.alpha = 1;
            currentKeysText.text = currentKeys.ToString();
        }

        void DisableKeysUI(InteractionList myValue)
        {
            currentKeys = 0;
            StartCoroutine(FadeOut());
        }

        IEnumerator FadeOut()
        {
            float alpha = canvasGroup.alpha;
            while (alpha > 0)
            {
                alpha -= Time.deltaTime / 2;
                if (alpha < 0) alpha = 0;
                canvasGroup.alpha = alpha;
                yield return new WaitForEndOfFrame();
            }
        }
    }
}

