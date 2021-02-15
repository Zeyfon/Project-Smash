using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PSmash.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] GameObject initialSelection =  null; 
        [SerializeField] MenuTab[] menuTabs;
        
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
            if (gameObject.activeInHierarchy) gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            if (eventSystem == null) return;
            StartCoroutine(SetInitialGeneralMenuSelection());
            UpdateSubMenus(initialSelection);
            foreach(MenuTab tab in menuTabs)
            {
                //print(tab.gameObject.name);
            }
        }

        private void Update()
        {
            //Will Perform the rest of the code only when a change in the eventSystem.curentSelectedGameObject has ocurred
            if (!HasMenuSelectionChanged())
                return;

            //The new selection is a Tab
            //Do according to this condition
            if (IsNewSelectionATab())
            {
                UpdateSubMenus(eventSystem.currentSelectedGameObject);
            }
            //The new selection is a button insside a subMenu
            //Do according to this condition
            else if(previousGameObject != null && previousGameObject.GetComponent<MenuTab>())
            {
                DisableInteractionWithRestOfTabs(previousGameObject);
            }
            previousGameObject = eventSystem.currentSelectedGameObject;
        }

        bool HasMenuSelectionChanged()
        {
            if (eventSystem.currentSelectedGameObject == null || previousGameObject != eventSystem.currentSelectedGameObject)
                return true;
            else
                return false;
        }

        bool IsNewSelectionATab()
        {
            foreach (MenuTab tab in menuTabs)
            {
                if (eventSystem.currentSelectedGameObject == tab.gameObject) return true;
            }
            return false;
        }

        void UpdateSubMenus(GameObject selectedTab)
        {
            foreach(MenuTab tab in menuTabs)
            {
                tab.DisableSubMenu();
                tab.SetInteractionCapacity(true);
            }
            foreach(MenuTab tab in menuTabs)
            {
                if (selectedTab == tab.gameObject)
                {
                    tab.EnableSubMenu();
                }
            }
        }

        void DisableInteractionWithRestOfTabs(GameObject subMenuTab)
        {
            foreach(MenuTab tab in menuTabs)
            {
                if (tab != subMenuTab.GetComponent<MenuTab>())
                {
                    tab.SetInteractionCapacity(false);
                    //print("Disabling " + tab.gameObject.name + " interaction capacity");
                }

            }
        }

        IEnumerator SetInitialGeneralMenuSelection()
        {
            eventSystem.SetSelectedGameObject(null);
            yield return null;
            eventSystem.SetSelectedGameObject(initialSelection);
            //print("General Tab Activated");
        }

        public void StarCollected()
        {
            currentStarsQuantity += 1;
        }
    }
}

