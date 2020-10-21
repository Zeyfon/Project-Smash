using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PSmash.Menus
{
    public class ControllerSelector : MonoBehaviour
    {
        [SerializeField] Image image = null;
        [SerializeField] Sprite controlLayoutSorute = null;

        EventSystem eventSystem;
        MainMenu mainMenu;

        private void Awake()
        {
            mainMenu = transform.parent.parent.parent.GetComponent<MainMenu>();
            eventSystem = GameObject.FindObjectOfType<EventSystem>();
        }

        
        //Will keep checking when the this.gameObject is selected by the eventSystem
        //in order to change the sprite shown on the image
        //and enable the image component in it if it is disabled
        void Update()
        {
            if (mainMenu.previousGameObject != eventSystem.currentSelectedGameObject &&
                eventSystem.currentSelectedGameObject == gameObject)
            {
                SetControlImage();
            }
        }

        void SetControlImage()
        {
            image.sprite = controlLayoutSorute;
            if (!image.enabled) image.enabled = true;
        }
    }

}
