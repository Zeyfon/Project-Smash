using System;
using UnityEngine;

namespace PSmash.Menus
{
    public class ContinueButton : MonoBehaviour
    {
        public static event Action OnMenuClose;
        public void CloseMenu()
        {
            if(OnMenuClose!= null)
            {
                OnMenuClose();
            }
        }
    }
}

