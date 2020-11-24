using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Menus
{
    public class QuitButton : MonoBehaviour
    {
        public void QuitGame()
        {
            print("Quitting Game");
            Application.Quit();
        }
    }
}

