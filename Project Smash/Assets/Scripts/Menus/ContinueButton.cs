using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Menus
{
    public class ContinueButton : MonoBehaviour
    {
        //Function as a Proxy to the method in Menus of CloseMenus
        public void PerformAction()
        {
            transform.parent.parent.parent.parent.GetComponent<Menus>().CloseMenus();
        }
    }
}

