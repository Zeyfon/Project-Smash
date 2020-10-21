using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PSmash.Menus
{
    public class ControlTab : MonoBehaviour
    {
        [SerializeField] Image image = null;
        
        EventSystem eventSystem;
        MainMenu mainMenu;

        private void Awake()
        {
            mainMenu = transform.parent.GetComponent<MainMenu>();
            eventSystem = GameObject.FindObjectOfType<EventSystem>();
        }
        
        //The Tab of the Control Layout will check when it is selected
        //to disable the image from the layouts
        void Update()
        {
            if (mainMenu.previousGameObject != eventSystem.currentSelectedGameObject &&
                eventSystem.currentSelectedGameObject == gameObject)
            {
                image.enabled = false;
                print("Disable Image");
            }
        }
    }
}

