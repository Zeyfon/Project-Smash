using UnityEngine;

namespace PSmash.Menus
{
    public class MainMenu : MonoBehaviour
    {
        // Start is called before the first frame update
        _Controller _controller;

        public delegate void MainMenuAction(bool isEnabled);
        public static event MainMenuAction OnMenuAction;

        private void Awake()
        {
            _controller = new _Controller();
        }
        void Start()
        {
            EnableMainMenu(false);
        }

        private void OnEnable()
        {
            _controller.Player.Enable();
            _controller.Player.ButtonStart.started += ctx => SwitchMainMenuState();
        }

        private void OnDisable()
        {
            _controller.Player.Disable();
            _controller.Player.ButtonStart.started += ctx => SwitchMainMenuState();
        }

        public void SwitchMainMenuState()
        {
            if (transform.GetChild(0).gameObject.activeInHierarchy == true)
            {
                EnableMainMenu(false);
                OnMenuAction(true);
            }
            else
            {
                EnableMainMenu(true);
                OnMenuAction(false);
            }
        }

        private void EnableMainMenu(bool isEnabled)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(isEnabled);
            }
        }
    }


}

