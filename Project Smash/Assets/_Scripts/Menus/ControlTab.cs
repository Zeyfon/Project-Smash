using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PSmash.Menus
{
    public class ControlTab : MonoBehaviour
    {
        [SerializeField] Image image = null;
        
        EventSystem eventSystem;

        private void Awake()
        {
            eventSystem = GameObject.FindObjectOfType<EventSystem>();
        }
        
        //The Tab of the Control Layout will check when it is selected
        //to disable the image from the layouts
        void Update()
        {
            bool isCurrentSelectionMyGameObject = eventSystem.currentSelectedGameObject == gameObject;

            if (isCurrentSelectionMyGameObject)
            {
                image.enabled = false;
            }
        }
    }
}

