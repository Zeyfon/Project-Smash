using PSmash.Stats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PSmash.LevelUpSystem
{
    public class SkillSlot : MonoBehaviour
    {

        [SerializeField] Skill skill = null;
        [SerializeField] Image image = null;
        [SerializeField] SkillSlot[] skillSlotsUnlockableOptions;
        [SerializeField] RequiredMaterial[] requiredMaterials;
        [SerializeField] UnlockableSkillPath[] unlockableSkillPaths;

        static Coroutine coroutine;
        // Start is called before the first frame update
        void Start()
        {
            image.sprite = skill.sprite;
        }

        /// <summary>
        /// Method used by the Button component in the same gameObject
        /// </summary>
        public void TryToUnlockSkill()
        {
            GetComponentInParent<UICraftingSystem>().TryToUnlockSkill(skill, this);
        }

        /// <summary>
        /// Inform about the options to be able to unlock
        /// </summary>
        /// <returns></returns>
        public SkillSlot[] GetSkillSlotsUnlockingOptions()
        {
            return skillSlotsUnlockableOptions;
        }

        /// <summary>
        /// Update the material to change between the 3 visuals aspects of the crafting system
        /// Locked, Unlockable and Unlocked
        /// </summary>
        /// <param name="material"></param>
        public void UpdateImageMaterial(Material material)
        {
            image.material = material;
        }

        /// <summary>
        /// To update the visuals of the links between Grey and White
        /// once it sets to white the links will not go back to dark again
        /// </summary>
        /// <param name="state"></param>
        public void UpdateLinks(string state)
        {
            foreach (UnlockableSkillPath unlockableSkillPath in unlockableSkillPaths)
            {
                unlockableSkillPath.link.UpdateColor(state);
            }
        }

        /// <summary>
        /// This method is to inform with a yes or no the Crafting System about having the necessary materials
        /// to be able to unlock
        /// if it does have the materials, it will first do the update of them and then inform the Crafting System
        /// about being able to do it.
        /// </summary>
        /// <param name="playerStats"></param>
        /// <returns></returns>
        public bool HaveNecessaryMaterials(BaseStats playerStats)
        {
            Dictionary<CraftingMaterialsList, int> craftingMaterials = new Dictionary<CraftingMaterialsList, int>();
            //print("Checking if having necessary materials in Skill Slot");
            foreach (RequiredMaterial requiredMaterial in requiredMaterials)
            {
                //print("The required material " + requiredMaterial.material);
                int materialQuantityPossesedByPlayer = playerStats.GetMaterialQuantity(requiredMaterial.material);
                if (materialQuantityPossesedByPlayer >= requiredMaterial.quantity)
                {
                    craftingMaterials.Add(requiredMaterial.material, requiredMaterial.quantity);
                    continue;
                }
                else
                {
                    return false;
                }
            }
            //print("The Player has all the required materials ");
            foreach (CraftingMaterialsList material in craftingMaterials.Keys)
            {
                print(material);
                playerStats.UpdateMaterialPossessedByPlayer(material, -craftingMaterials[material]);
            }
            return true;
        }

        /// <summary>
        /// This will update the Description Window with the information about this skillSlot
        /// (Description, the materials needed and the quantity for them to be unlocked
        /// </summary>
        /// <param name="descriptionWindow"></param>
        public void UpdateDescriptionWindow(UIDescriptionWindow descriptionWindow)
        {
            //print("Updating Description Window");
            foreach (UICraftingMaterial craftingMaterial in descriptionWindow.craftinMaterials)
            {
                //print("Checking if is required  " + craftingMaterial.material.ToString());
                bool isMaterial = false;
                foreach (RequiredMaterial requiredMaterial in requiredMaterials)
                {
                    if (craftingMaterial.material == requiredMaterial.material)
                    {
                        isMaterial = true;
                        craftingMaterial.gameObject.SetActive(true);
                        craftingMaterial.UpdateValue(requiredMaterial.quantity);
                    }
                }
                if (!isMaterial)
                {
                    //print("It does not require  " + craftingMaterial.material.ToString());
                    craftingMaterial.gameObject.SetActive(false);
                }
            }
            descriptionWindow.text.text = skill.description;
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
                //cr_Running = false;
                //print("Stoping Coroutine");
                //Debug.Break();
            }

            descriptionWindow.SetWindowAlphaToCero();
            coroutine = StartCoroutine(DescriptionWindowFadeIn(descriptionWindow));

        }

        IEnumerator DescriptionWindowFadeIn(UIDescriptionWindow descriptionWindow)
        {
            //cr_Running = true;
            //print("Wants to Fade In Description Window");
            yield return descriptionWindow.WaitingToAppear();
            yield return descriptionWindow.FadeIn();

            //print("FadeInComplete");
            coroutine = null;
            //cr_Running = false;
        }


        [System.Serializable]
        public class RequiredMaterial
        {
            public CraftingMaterialsList material;
            public int quantity;
        }

        [System.Serializable]
        public class UnlockableSkillPath
        {
            public UICraftingSystemLink link;
        }

    }

}
