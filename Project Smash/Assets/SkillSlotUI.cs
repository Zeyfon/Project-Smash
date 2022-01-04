using PSmash.CraftingSystem;
using UnityEngine;
using UnityEngine.UI;

namespace PSmash.UI.CraftingSytem
{
    public class SkillSlotUI : MonoBehaviour
    {

        [SerializeField] UnlockableSkillPath[] availablePathsOnceUnlocked;
        [SerializeField] Material skillUnlockableMaterial = null;
        [SerializeField] Material yellowMaterial = null;

        [SerializeField] Image skillSlotImage = null;
        [SerializeField] SkillSlot slot = null;


        [System.Serializable]
        public class UnlockableSkillPath
        {
            public Link link;
        }

        private void OnEnable()
        {
            slot.OnSkillPanelUpdate += UpdateSkillslotSate;
        }


        private void OnDisable()
        {
            slot.OnSkillPanelUpdate -= UpdateSkillslotSate;
        }

        private void UpdateSkillslotSate(SkillSlot skillSlot)
        {
            if (slot.IsUnlocked())
            {
                UpdateLinks("White");
                if(skillSlot.gameObject != gameObject)
                    SetRingToNull();
            }
            else if(!slot.IsUnlocked() && slot.CanBeUnlocked())
            {
                SetRightToWhite();
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

        public void SetRingToYellow()
        {
            //print(gameObject.name + "  yellow ring");
            Image image = transform.GetChild(0).GetComponent<Image>();
            image.material = yellowMaterial;
            image.enabled = true;
        }

        public void SetRingToWhatIsNecessary()
        {
            if (slot.IsUnlocked())
            {
                SetRingToNull();
                UpdateLinks("White");
            }
            else if (!slot.IsUnlocked() && slot.CanBeUnlocked())
            {
                SetRightToWhite();
            }
            else
            {
                SetRingToNull();
            }
        }

#if UNITY_EDITOR
        private void Update()
        {
            if (Application.IsPlaying(gameObject)) return;
            if (slot.GetSkill() != null)
            {
                skillSlotImage.sprite = slot.GetSkill().GetSprite();
            }
            else
            {
                print("Not setting skill");
            }
        }
#endif
    }

}
