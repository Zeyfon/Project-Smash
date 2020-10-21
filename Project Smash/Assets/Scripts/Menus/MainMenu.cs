using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PSmash.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] Text maxStarsText = null;
        [SerializeField] Text currentStarsText = null;
        [SerializeField] Transform[] menuTabs = null;
        
        //This variable is used by different UI elements that need to know
        //the previous eventSystem.currentGameObjectSelected in order
        //to compare themselves with the current eventSyste.currentGameobjectSelected
        //to perform their diverse actions.
        public GameObject previousGameObject;

        EventSystem eventSystem;

        int currentStarsQuantity = 0;

        private void Awake()
        {
            eventSystem = GameObject.FindObjectOfType<EventSystem>();
        }
        void Start()
        {
            maxStarsText.text = PSmash.Items.Star.activeStarsOnSceneQuantity.ToString();
            if (gameObject.activeInHierarchy) gameObject.SetActive(false);
        }

        private void DisableSubMeus()
        {
            foreach (Transform tab in menuTabs)
            {
                tab.GetChild(2).transform.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            //Only let pass when the selection has changed from the previous one
            //in the EventSystem
            if (previousGameObject == eventSystem.currentSelectedGameObject || 
                eventSystem.currentSelectedGameObject ==null) return;
            
            if (IsSelectionATab())
            {
                //Enable the button Component in all the Tabs
                //to let the player change between them
                EnableTabsButtonComponent();

                if (previousGameObject != null && IsPreviousSelectionATab())
                {
                    //Disable the subMenu from the previosuGameObject
                    previousGameObject.transform.GetChild(2).gameObject.SetActive(false);
                }
                //Enable the subMenu from the previousGameObject
                eventSystem.currentSelectedGameObject.transform.GetChild(2).gameObject.SetActive(true);
            }
            else
            {
                //This foreach will disable the button component in all the tabs
                //except from the one whose submenu is currently active
                foreach(Transform tab in menuTabs)
                {
                    if (tab == eventSystem.currentSelectedGameObject.transform.parent.parent)
                    {
                        //if the tab is the owner of the submenu will continue the foreach loop
                        //without disabling the button component
                        continue;
                    }
                    tab.GetComponent<Button>().enabled = false;
                }
            }
        }

        //Since previousGameObject is used by different UI Elements is updated in LateUpdate
        //To let the Update Methods in those entities to work properly without having 
        //running issues of this variable being updated before finishing their tasks.
        private void LateUpdate()
        {
            previousGameObject = eventSystem.currentSelectedGameObject;
        }

        void EnableTabsButtonComponent()
        {
            foreach(Transform tab in menuTabs)
            {
                tab.GetComponent<Button>().enabled = true;
            }
        }

        bool IsSelectionATab()
        {
            foreach(Transform tab in menuTabs)
            {
                if (eventSystem.currentSelectedGameObject == tab.gameObject) return true;
            }
            return false;
        }

        bool IsPreviousSelectionATab()
        {
            foreach(Transform tab in menuTabs)
            {
                if (tab == previousGameObject.transform)
                {
                    print("previous is a tab");
                    return true;
                }
            }
            return false;
        }

        private void OnEnable()
        {
            currentStarsText.text = currentStarsQuantity.ToString();
            SetInitialMainMenuState();
            StartCoroutine(SetInitialTabSelection());
        }

        void SetInitialMainMenuState()
        {
            DisableSubMeus();
            EnableTabsButtonComponent();
        }

        //A coroutine is used here in order to let a frame pass between 
        //setting the selectedGameObject to null
        //and set it to another gameObject without affecting other elements
        // in the script

        IEnumerator SetInitialTabSelection()
        {
            eventSystem.SetSelectedGameObject(null);
            yield return null;
            eventSystem.SetSelectedGameObject(menuTabs[0].gameObject);
            menuTabs[0].transform.GetChild(2).gameObject.SetActive(true);
            print("General Tab Activated");
        }

        public void StarCollected()
        {
            currentStarsQuantity += 1;
        }
    }
}

