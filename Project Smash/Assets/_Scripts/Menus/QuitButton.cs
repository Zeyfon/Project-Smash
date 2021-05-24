using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.SceneManagement;
using PSmash.Attributes;

namespace PSmash.Menus
{
    public class QuitButton : MonoBehaviour
    {
        public void QuitGame()
        {
            print("Quitting Game");
            FindObjectOfType<PlayerHealth>().gameObject.layer = LayerMask.NameToLayer("Dead");
            FindObjectOfType<SavingWrapper>().QuitGame();
            GetComponentInParent<MainMenu>().CloseMainMenu();
        }
    }
}

