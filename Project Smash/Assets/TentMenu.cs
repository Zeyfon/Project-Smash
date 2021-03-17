using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Saving;
using System;
using UnityEngine.EventSystems;
using PSmash.InputSystem;

public class TentMenu : MonoBehaviour
{
    [SerializeField] GameObject initialSelection = null;

    public static event Action OnMenuOpen;

    public static event Action OnMenuClose;
    _Controller _controller;
    // Start is called before the first frame update
    void Start()
    {
        _controller = new _Controller();
        CloseMenu();
    }

    private void OnEnable()
    {
        Tent.OnTentMenuOpen += OpenTentMenu;
    }

    /// <summary>
    /// The method called by the event to open the Tent Menu.
    /// The signal comes from the InputHandler inside the Player for now.
    /// </summary>

    public void OpenTentMenu()
    {
        //Enable Menu
        print("Open Tent Menu");
        SetChildObjects(true);
        StartCoroutine(EnableControl());
        StartCoroutine(InitializeSelection());
    }

    IEnumerator EnableControl()
    {
        yield return new WaitForEndOfFrame();
        _controller.UI.Enable();
        _controller.UI.Cancel.performed += ctx => BacktrackMenu();
        _controller.UI.ButtonStart.performed += ctx => CloseAllMenus();
    }

    private void CloseAllMenus()
    {
        CloseTentMenu();
    }

    private void BacktrackMenu()
    {
        CloseTentMenu();
    }

    void CloseTentMenu()
    {
        OnMenuClose();
        CloseMenu();
        _controller.UI.Disable();
        _controller.UI.Cancel.performed -= ctx => BacktrackMenu();
    }

    IEnumerator InitializeSelection()
    {
        EventSystem eventSystem = FindObjectOfType<EventSystem>();
        eventSystem.SetSelectedGameObject(null);
        yield return null;
        eventSystem.SetSelectedGameObject(initialSelection);
    }

    /// <summary>
    /// Open and Close menu actions
    /// </summary>

    void CloseMenu()
    {
        SetChildObjects(false);
    }

    void OpenMenu()
    {
        SetChildObjects(true);
    }

    private void SetChildObjects(bool isEnabled)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(isEnabled);
        }
    }


    /// <summary>
    /// TODO. This part must be taken out as is a method used by the button in the Tent Menu to open the Crafting Menu
    /// </summary>
    public void OpenCraftingMenu()
    {
        if (OnMenuOpen != null)
        {
            OnMenuOpen();
            CloseMenu();
            _controller.UI.Disable();
            _controller.UI.Cancel.performed -= ctx => BacktrackMenu();
        }
    }
}
