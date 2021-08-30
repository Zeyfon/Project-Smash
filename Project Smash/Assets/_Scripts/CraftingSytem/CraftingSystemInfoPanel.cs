using TMPro;
using UnityEngine;

namespace PSmash.CraftingSystem
{
    public class CraftingSystemInfoPanel : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI nameText = null;
        [SerializeField] TextMeshProUGUI descriptionText = null;

        public void UpdateInfoPanelWithSkillInfo(SkillSlot slot)
        {
            descriptionText.text = slot.GetSkill().GetDescription();
            nameText.text = slot.GetSkill().GetItem().displayName;
            GetComponentInChildren<ToolTipCraftingItemsHandler>().SetSkillSlotInfo(slot);
        }
    }

}
