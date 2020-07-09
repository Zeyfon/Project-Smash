using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using UnityEngine;

public class Menus : MonoBehaviour
{
    [SerializeField] GameObject bootsMenu;

    _Controller _controller;
    // Start is called before the first frame update
    void Awake()
    {
        _controller = new _Controller();
    }

    private void OnEnable()
    {
        EventManager.PlayerGotBoots += PlayerGotBoots;
        _controller.UI.ButtonA.performed += IBindCtx => ButtonAPressed();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void PlayerGotBoots()
    {
        bootsMenu.SetActive(true);
        StartCoroutine(EnableController());
    }

    IEnumerator EnableController()
    {
        float timer = 0;
        while (timer < 3)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        _controller.UI.Enable();
    }
    
    void ButtonAPressed()
    {
        GameObject.FindObjectOfType<EventManager>().GameWillUnpause();
        bootsMenu.SetActive(false);
    }
}
