using PSmash.Inventories;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PSmash.UI.CraftingSytem;


namespace PSmash.CraftingSystem
{
    public class SkillSlot : MonoBehaviour
    {
        [Header("CONFIG")]
        [SerializeField] Skill skill = null;
        [SerializeField] Image skillSlotImage = null;
        [SerializeField] SkillSlot[] unlockableBySkillSlots;
        [SerializeField] CraftingItemSlot[] requiredCraftingItems;
        [SerializeField] UnlockableSkillPath[] availablePathsOnceUnlocked;

        [SerializeField] Material saturationCeroMaterial = null;

        [Header("STATE")]
        [SerializeField]bool isUnlocked = false;
        // Start is called before the first frame update
        void Start()
        {
            skillSlotImage.sprite = skill.GetSprite();
        }

        public Item GetItem()
        {
            return skill.GetItem();
        }

        public Skill GetSkill()
        {
            return skill;
        }


        public void Unlock()
        {
            isUnlocked = true;
        }

        public CraftingItemSlot[] GetRequiredCraftingItems()
        {
            //print(requiredCraftingItems.Length);
            return requiredCraftingItems;
        }
        /// <summary>
        /// Inform about the options to be able to unlock
        /// </summary>
        /// <returns></returns>
        public SkillSlot[] GetSkillSlotsUnlockingOptions()
        {
            return unlockableBySkillSlots;
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



        //////////////// UI//////////////////////////////////////////
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
        bool IsUnlockable()
        {
            foreach (SkillSlot slot in unlockableBySkillSlots)
            {
                if (slot.GetIsUnlocked())
                {
                    return true;
                }
            }
            if (unlockableBySkillSlots.Length == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// The Update in the UI bases on the state of the skillSlot.
        /// Saturation 1
        /// </summary>
        void Unlocked()
        {
            //print(skill.name + "  is unlocked");
            UpdateSkillSlotVisualState(null);
            UpdateLinks("White");
        }
        /// <summary>
        /// The Update in the UI bases on the state of the skillSlot.
        /// Saturation 0 with White Ring
        /// </summary>
        void Unlockable()
        {
            //print(skill.name + " is unlockable");
            UpdateSkillSlotVisualState(saturationCeroMaterial);
            UpdateLinks("Dark");
            EnableWhiteRing();
        }

        /// <summary>
        /// The Update in the UI bases on the state of the skillSlot.
        /// Saturation 0 without White Ring
        /// </summary>
        void Locked()
        {
            //print(skill.name + " is locked");
            UpdateSkillSlotVisualState(saturationCeroMaterial);
            UpdateLinks("Dark");
            DisableWhiteRing();
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

        void DisableWhiteRing()
        {
            GetComponentInChildren<Image>().enabled = false ;
        }

        void EnableWhiteRing()
        {
            GetComponentInChildren<Image>().enabled = true;

        }


        public void SetRingMaterial(Material yellowMaterial)
        {
            Image image = transform.GetChild(0).GetComponent<Image>();
            image.enabled = true;
            image.material = yellowMaterial;
        }

        //public object CaptureState()
        //{
        //    return IsUnlocked();
        //}

        //public void RestoreState(object state)
        //{
        //    isUnlocked = (bool)state;
        //    VisualUpdate();
        //}
    }

}
