using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PSmash.Temporal
{
    public class ControllerSelector : MonoBehaviour
    {
        [SerializeField] Image image = null;
        [SerializeField] Sprite controlLayoutSprite = null;

        EventSystem eventSystem;

        private void Awake()
        {
            eventSystem = GameObject.FindObjectOfType<EventSystem>();
        }

        void Update()
        {
            if (eventSystem.currentSelectedGameObject == gameObject)
            {
                //print(gameObject.name + " Setting image");
                SetControlSubMenuImage();
            }
        }

        void SetControlSubMenuImage()
        {
            if (controlLayoutSprite == null)
            {
                image.enabled = false;
                return;
            }
            image.sprite = controlLayoutSprite;
            if (!image.enabled) image.enabled = true;
        }
    }

}
