using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PSmash.Temporal
{
    public class ControlSubMenu : MonoBehaviour
    {
        [SerializeField] Button controllerButton = null;
        [SerializeField] Button keyboardButton = null;
        [SerializeField] Image image = null;
        [SerializeField] Sprite controlSprite = null;
        [SerializeField] Sprite keyboardSprite = null;

        EventSystem eventSystem;

        private void Start()
        {
            eventSystem = GameObject.FindObjectOfType<EventSystem>();
        }
        private void OnEnable()
        {
            image.enabled = false;
        }

        void Update()
        {
            
            if (eventSystem.currentSelectedGameObject == controllerButton.gameObject)
            {
                SetControllerSprite();
            }
            else if(eventSystem.currentSelectedGameObject == keyboardButton.gameObject)
            {
                SetKeyboardSprite();
            }
            //else
            //{
            //    image.enabled = false;
            //}
        }

        void SetControllerSprite()
        {
            image.sprite = controlSprite;
            image.enabled = true;
        }

        void SetKeyboardSprite()
        {
            image.sprite = keyboardSprite;
            image.enabled = true;
        }
    }

}
