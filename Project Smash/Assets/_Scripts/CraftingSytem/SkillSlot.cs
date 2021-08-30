using PSmash.Inventories;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PSmash.UI.CraftingSytem;

namespace PSmash.CraftingSystem
{
    [ExecuteAlways]
    public class SkillSlot : MonoBehaviour
    {
        //CONFIG
        [Header("CONFIG")]
        [SerializeField] Skill skill = null;
        [SerializeField] Image skillSlotImage = null;
        [SerializeField] SkillSlot[] unlockableBySkillSlotsOptions;
        [SerializeField] CraftingItemSlot[] requiredCraftingItems;
        [SerializeField] UnlockableSkillPath[] availablePathsOnceUnlocked;
        [SerializeField] Material saturationCeroMaterial = null;

        [Header("STATE")]
        [SerializeField]bool isUnlocked = false;


        //STATE
        [System.Serializable]
        public class CraftingItemSlot
        {
            public CraftingItem item;
            public int number;
        }

        [System.Serializable]
        public class UnlockableSkillPath
        {
            public Link link;
        }

        //INITIALIZE

        // Start is called before the first frame update
        void Start()
        {
            SetSkillSlotImage();
        }



        //////////////////////////////////////////////////////////////////////PUBLIC/////////////////////////////////////////////////////////////////////

        public Item GetItem()
        {
            return skill.GetItem();
        }

        public Skill GetSkill()
        {
            return skill;
        }

        public CraftingItemSlot[] GetRequiredCraftingItems()
        {
            return requiredCraftingItems;
        }
        /// <summary>
        /// Inform about the options to be able to unlock
        /// </summary>
        /// <returns></returns>
        public SkillSlot[] GetSkillSlotsUnlockingOptions()
        {
            return unlockableBySkillSlotsOptions;
        }

        /// <summary>
        /// Update the material to change between the 3 visuals aspects of the crafting system
        /// Locked, Unlockable and Unlocked
        /// </summary>
        /// <param name="material"></param>
        public void UpdateImageMaterial(Material material)
        {
            skillSlotImage.material = material;
        }

        public void UpdateSkillSlotVisualState(Material material)
        {
            skillSlotImage.material = material;
            //ringImage.enabled = isRingEnabled;
        }

        public Dictionary<CraftingItem, int> GetCraftingItemsRequirement()
        {
            Dictionary<CraftingItem, int> requiredCraftingItems = new Dictionary<CraftingItem, int>();

            foreach (CraftingItemSlot requiredCraftingMaterial in this.requiredCraftingItems)
            {
                //print("Adding  " + requiredCraftingMaterial.material);
                requiredCraftingItems.Add(requiredCraftingMaterial.item, requiredCraftingMaterial.number);
            }
            //print(requiredCraftingItems.Count);
            //print("Got the Required Materials " + requiredMaterials);
            return requiredCraftingItems;
        }

        public void VisualUpdate()
        {
            if (GetIsUnlocked())
            {
                UpdateLinks("White");
                Unlocked();
            }
            else if (IsUnlockable())
            {
                UpdateLinks("White");
                Unlockable();
            }
            else
            {
                UpdateLinks("Dark");
                Locked();
            }
        }

        /// <summary>
        /// Checks if the skillSlot has been unlocked
        /// </summary>
        /// <returns></returns>
        public bool GetIsUnlocked()
        {
            return isUnlocked;
        }

        public void SetIsUnlocked(bool isUnlocked)
        {
            this.isUnlocked = isUnlocked;
        }

        /// <summary>
        /// Checks if the skillSlot is unlockable
        /// </summary>
        /// <returns></returns>
        public bool IsUnlockable()
        {
            if (isUnlocked)
                return false;

            foreach (SkillSlot slot in unlockableBySkillSlotsOptions)
            {
                if (slot.GetIsUnlocked())
                {
                    return true;
                }
            }
            if (unlockableBySkillSlotsOptions.Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }



        /// <summary>
        /// To update the visuals of the links between Grey and White
        /// once it sets to white the links will not go back to dark again
        /// </summary>
        /// <param name="state"></param>
        public void UpdateLinks(string state)
        {
            foreach (UnlockableSkillPath unlockableSkillPath in availablePathsOnceUnlocked)
            {
                //unlockableSkillPath.link.UpdateColor(state);
                if (state == "White")
                {
                    unlockableSkillPath.link.GetComponent<Image>().color = Color.white;
                }
                else
                {
                    unlockableSkillPath.link.GetComponent<Image>().color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
                }
            }
        }

        public void SetRightToWhite()
        {
            //print(gameObject.name + "  white ring");
            Image image = transform.GetChild(0).GetComponent<Image>();
            image.material = null;
            image.enabled = true;

        }

        public void SetRingToNull()
        {
            //print(gameObject.name + "  no ring");
            Image image = transform.GetChild(0).GetComponent<Image>();
            image.enabled = false;
        }

        public void SetRingToYellow(Material yellowMaterial)
        {
            //print(gameObject.name + "  yellow ring");
            Image image = transform.GetChild(0).GetComponent<Image>();
            image.material = yellowMaterial;
            image.enabled = true;
        }


        //////////////////////////////////////////////////////////////////////PRIVATE/////////////////////////////////////////////////////////////////////

        /// <summary>
        /// The Update in the UI bases on the state of the skillSlot.
        /// Saturation 1
        /// </summary>
        void Unlocked()
        {
            UpdateSkillSlotVisualState(null);
            UpdateLinks("White");
        }
        /// <summary>
        /// The Update in the UI bases on the state of the skillSlot.
        /// Saturation 0 with White Ring
        /// </summary>
        void Unlockable()
        {
            UpdateSkillSlotVisualState(saturationCeroMaterial);
            UpdateLinks("Dark");
        }

        /// <summary>
        /// The Update in the UI bases on the state of the skillSlot.
        /// Saturation 0 without White Ring
        /// </summary>
        void Locked()
        {
            UpdateSkillSlotVisualState(saturationCeroMaterial);
            UpdateLinks("Dark");
        }

        void SetSkillSlotImage()
        {
            skillSlotImage.sprite = skill.GetSprite();
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (skill != null)
            {
                SetSkillSlotImage();
            }
            else
            {
                print("Not setting skill");
            }
        }
#endif

    }

}
