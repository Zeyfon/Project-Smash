using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSmash.Menus
{
    public class MainMenuSelector : MonoBehaviour
    {
        [SerializeField] GameObject initialSelection =  null; 
        [SerializeField] MenuTab[] menuTabs;
        
        //This variable is used by different UI elements that need to know
        //the previous eventSystem.currentGameObjectSelected in order
        //to compare themselves with the current eventSyste.currentGameobjectSelected
        //to perform their diverse actions.
        GameObject previousGameObject;
        EventSystem eventSystem;

        int currentStarsQuantity = 0;

        private void Start()
        {
            eventSystem = GameObject.FindObjectOfType<EventSystem>();
            if(eventSystem == null)
            {
                Debug.LogWarning("Main Menu did not find an eventSystem");
            }
        }

        private void OnEnable()
        {
            //print("Trying to open main menu");
            if (eventSystem == null)
            {
                eventSystem = GameObject.FindObjectOfType<EventSystem>();
                if (eventSystem == null)
                {
                    return;
                }
            }
            StartCoroutine(SetInitialGeneralMenuSelection());
            //print("Main menu enabled");
            UpdateSubMenu(initialSelection);
        }

        IEnumerator SetInitialGeneralMenuSelection()
        {
            eventSystem.SetSelectedGameObject(null);
            yield return null;
            eventSystem.SetSelectedGameObject(initialSelection);
        }

        private void Update()
        {
            if (!HasMenuSelectionChanged())
                return;

            if (IsCurrentSelectionATab())
            {
                UpdateSubMenu(eventSystem.currentSelectedGameObject);
            }
            else if(previousGameObject != null && previousGameObject.GetComponent<MenuTab>())
            {
                DisableInteractionWithRestOfTabs(previousGameObject);
            }
            previousGameObject = eventSystem.currentSelectedGameObject;
        }

        //SubFunction in Update
        bool HasMenuSelectionChanged()
        {
            if (eventSystem.currentSelectedGameObject == null || previousGameObject != eventSystem.currentSelectedGameObject)
                return true;
            else
                return false;
        }

        //SubFunction in Update
        bool IsCurrentSelectionATab()
        {
            foreach (MenuTab tab in menuTabs)
            {
                if (eventSystem.currentSelectedGameObject == tab.gameObject) return true;
            }
            return false;
        }

        //SubFunction in Update
        void UpdateSubMenu(GameObject selectedTab)
        {
           // print("Set Initial Selection Main menu");
            foreach(MenuTab tab in menuTabs)
            {
                tab.DisableSubMenu();
                tab.SetInteractionCapacity(true);
            }
            foreach(MenuTab tab in menuTabs)
            {
                if (selectedTab == tab.gameObject)
                {
                    //print("enabled tab  " + tab.gameObject.name);
                    tab.EnableSubMenu();
                }
            }
        }

        //SubFunction in Update
        void DisableInteractionWithRestOfTabs(GameObject subMenuTab)
        {
            foreach(MenuTab tab in menuTabs)
            {
                if (tab != subMenuTab.GetComponent<MenuTab>())
                {
                    tab.SetInteractionCapacity(false);
                }
            }
        }
    }
}

