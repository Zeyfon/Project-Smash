using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillPanel : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Material yellowMaterial = null;
    GameObject currentSkillSelected;
    GameObject previousSkillSelected;

    EventSystem eventSystem;
    void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();
    }

    // Update is called once per frame
    void OnEnable()
    {
        if (eventSystem != null)
        {
            if (previousSkillSelected != null && currentSkillSelected != previousSkillSelected)
            {
                print("Set Previous Selection");
                eventSystem.SetSelectedGameObject(previousSkillSelected);
                SetSelectorRing(previousSkillSelected, null);
            }

            else
            {
                print("Set Initial Selection");
                if (currentSkillSelected == null)
                    Debug.LogWarning("The skillSlot selected is empty");
                eventSystem.SetSelectedGameObject(currentSkillSelected);
                SetSelectorRing(currentSkillSelected, previousSkillSelected);
            }
        }
    }

    private void Update()
    {
        if (eventSystem == null)
            return;
        if (previousSkillSelected != eventSystem.currentSelectedGameObject)
        {
            SetSelectorRing(eventSystem.currentSelectedGameObject, previousSkillSelected);
            previousSkillSelected = eventSystem.currentSelectedGameObject;
        }
    }

    void SetSelectorRing(GameObject currentSelection, GameObject previousSelection)
    {
        print("Updating current " + currentSelection.name);
        Image currentSkillSlotImage = currentSelection.GetComponentInChildren<Image>();
        currentSkillSlotImage.material = yellowMaterial;
        currentSkillSlotImage.enabled = true;
        if (previousSelection == null)
            return;
        //if skillSlot is unlockable get the white ring
        //else
            //disable rign
        previousSelection.GetComponentInChildren<Image>().material = null;

    }
}
