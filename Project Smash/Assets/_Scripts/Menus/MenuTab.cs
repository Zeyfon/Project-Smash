using UnityEngine;
using UnityEngine.UI;

namespace PSmash.Menus
{
    public class MenuTab : MonoBehaviour
    {

        [SerializeField] GameObject mySubMenu = null;

        public void DisableSubMenu()
        {
            //print("Disabling " + mySubMenu.name);
            mySubMenu.SetActive(false);
        }

        public void EnableSubMenu()
        {
            //print("Enabling " + mySubMenu.name);
            mySubMenu.SetActive(true);
        }

        public void SetInteractionCapacity(bool state)
        {
            GetComponent<Button>().enabled = state;
        }
    }
}

