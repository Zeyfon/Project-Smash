using UnityEngine;
using PSmash.InputSystem;

namespace PSmash.Menus
{
    public class Menus : MonoBehaviour
    {
        //[SerializeField] Transform weaponsTransform = null;
        [SerializeField] GameObject status = null;        public delegate void MenusClosed(bool state);
        public static event MenusClosed OnMenusClosed;
        GameObject statusMenuClone;
        _Controller _controller;
        // Start is called before the first frame update
        void Awake()
        {
            statusMenuClone = Instantiate(status, transform);
            _controller = new _Controller();
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
            _controller.UI.Disable();
            statusMenuClone.transform.GetChild(0).gameObject.SetActive(false);
            OnMenusClosed(true);
            Time.timeScale = 1;
        }
    }
}

