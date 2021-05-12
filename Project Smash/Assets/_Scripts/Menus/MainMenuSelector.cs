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
            UpdateMenuToNewTabSelection(initialSelection);
        }

        IEnumerator SetInitialGeneralMenuSelection()
        {
            eventSystem.SetSelectedGameObject(null);
            yield return null;
            eventSystem.SetSelectedGameObject(initialSelection);
        }

        void Update()
        {
            if (!HasMenuSelectionChanged())
                return;

            if (IsNewSelectionATab())
            {
                UpdateMenuToNewTabSelection(eventSystem.currentSelectedGameObject);
            }
            else if (HasNewSelectionPassedFromATabToAButton())
            {
                DisableInteractionCapacityOfOtherTabs(previousGameObject);
            }
            previousGameObject = eventSystem.currentSelectedGameObject;
        }

        /// <summary>
        /// Tells you when the selection has changed. Regardless if is a tab or button
        /// </summary>
        /// <returns></returns>
        bool HasMenuSelectionChanged()
        {
            if (eventSystem.currentSelectedGameObject == null || previousGameObject != eventSystem.currentSelectedGameObject)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Tells you if the new selection is a tab or not
        /// </summary>
        /// <returns></returns>
        bool IsNewSelectionATab()
        {
            foreach (MenuTab tab in menuTabs)
            {
                if (eventSystem.currentSelectedGameObject == tab.gameObject) return true;
            }
            return false;
        }

        /// <summary>
        /// 1. Disable all submenus components for the current selection
        /// 2. Enable all tabs interaction capacity
        /// 3. Enables the current tab selection sub menu
        /// </summary>
        /// <param name="selectedTab"></param>
        void UpdateMenuToNewTabSelection(GameObject selectedTab)
        {

            DisableAllSubMenus();
            EnableInteractionCapacityForAllTabs();
            EnableThisTabMenu(selectedTab);
        }


        /// <summary>
        /// 1. Disable the submenu of the tab and enables the interaction capacity of it for the event system
        /// </summary>
        private void DisableAllSubMenus()
        {
            foreach (MenuTab tab in menuTabs)
            {
                tab.DisableSubMenu();
                tab.SetInteractionCapacity(true);
            }
        }

        /// <summary>
        /// 2. Enable all tabs interaction capacity
        /// </summary>
        private void EnableInteractionCapacityForAllTabs()
        {
            foreach (MenuTab tab in menuTabs)
            {
                tab.SetInteractionCapacity(true);
            }
        }

        /// <summary>
        /// 3. Enables the current tab selection sub menu
        /// </summary>
        /// <param name="selectedTab"></param>
        private void EnableThisTabMenu(GameObject selectedTab)
        {
            foreach (MenuTab tab in menuTabs)
            {
                if (selectedTab == tab.gameObject)
                {
                    tab.EnableSubMenu();
                }
            }
        }

        /// <summary>
        /// Tells you if the new selection is passing from a tab to a button
        /// </summary>
        /// <returns></returns>
        bool HasNewSelectionPassedFromATabToAButton()
        {
            return previousGameObject != null && previousGameObject.GetComponent<MenuTab>();
        }

        /// <summary>
        /// Disable the interaction capacity of all the others tabs of the current selected one
        /// </summary>
        /// <param name="subMenuTab"></param>
        void DisableInteractionCapacityOfOtherTabs(GameObject subMenuTab)
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

