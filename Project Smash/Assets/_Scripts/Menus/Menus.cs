using UnityEngine;
using PSmash.InputSystem;

namespace PSmash.Menus
{
    public class Menus : MonoBehaviour
    {
        //[SerializeField] Transform weaponsTransform = null;
        [SerializeField] GameObject status = null;        
        [SerializeField] GameObject craftingSystem = null;

        public delegate void MenusClosed(bool state);
        public static event MenusClosed OnMenusClosed;
        GameObject statusMenuClone;
        GameObject craftingSystemClone;
        _Controller _controller;
        // Start is called before the first frame update
        void Awake()
        {
            statusMenuClone = Instantiate(status, transform);
            craftingSystemClone = Instantiate(craftingSystem, transform);
            _controller = FindObjectOfType<InputHandler>().GetController();
            if (_controller == null) Debug.LogWarning("Controller was not found");
            //_controller = new _Controller();
        }

        private void OnEnable()
        {
            _controller.UI.Cancel.performed += ctx => MenuBackTrack();
            PSmash.InputSystem.InputHandler.OnPlayerStartButtonPressed += EnableMenus;
            PSmash.Items.Star.OnStarCollected += StarCollected;
        }

        private void OnDisable()
        {
            _controller.UI.Cancel.performed -= ctx => MenuBackTrack();
            PSmash.InputSystem.InputHandler.OnPlayerStartButtonPressed -= EnableMenus;
            PSmash.Items.Star.OnStarCollected -= StarCollected;
        }

        void EnableMenus()
        {
            _controller.UI.Enable();
            statusMenuClone.transform.GetChild(0).gameObject.SetActive(true);
            Time.timeScale = 0;
        }

        void ButtonStartPressed()
        {
            CloseMenus();
        }

        void MenuBackTrack()
        {
            //print("Backtracking in menus  ");
            //if no menu above close the menus
                CloseMenus();
            //Go One menu up
        }

        void StarCollected()
        {
            statusMenuClone.GetComponent<MainMenu>().StarCollected();
        }

        public  void CloseMenus()
        {
            //OnMenusClosed(true);

            _controller.UI.Disable();
            Time.timeScale = 1;

            statusMenuClone.transform.GetChild(0).gameObject.SetActive(false);

        }
    }
}

