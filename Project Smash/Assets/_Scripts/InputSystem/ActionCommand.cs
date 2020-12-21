using UnityEngine;

namespace PSmash.InputSystem
{
    public class ActionCommand : MonoBehaviour
    {

        public ButtonList myButton;

        public void ChangeMyActionButton(ICommand com, ButtonList newButton)
        {
            myButton = newButton;
        }
    }
}

