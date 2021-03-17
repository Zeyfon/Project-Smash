using UnityEngine;
using PSmash.InputSystem;

namespace PSmash.Menus
{
    /// <summary>
    /// Its only function is to Instantiante and keep organize all the menus the game will use.
    /// </summary>
    public class Menus : MonoBehaviour
    {
        //[SerializeField] Transform weaponsTransform = null;
        [SerializeField] GameObject status = null;        
        [SerializeField] GameObject craftingSystem = null;
        [SerializeField] GameObject tentMenu = null;

        //public delegate void MenusClosed(bool state);
        //public static event MenusClosed OnMenusClosed;

        //_Controller _controller;
        // Start is called before the first frame update
        void Awake()
        {
            Instantiate(status, transform);
            Instantiate(craftingSystem, transform);
            Instantiate(tentMenu, transform);

            //_controller = FindObjectOfType<InputHandler>().GetController();
            //if (_controller == null) Debug.LogWarning("Controller was not found");
        }

    }
}

