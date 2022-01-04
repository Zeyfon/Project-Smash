using UnityEngine;

namespace PSmash.CraftingSystem
{
    /// <summary>
    /// Only use is the OnClick UnityEvent to try to unlock this skill in the CraftingSystem
    /// </summary>
    public class SkillSlotButton : MonoBehaviour
    {
        public void TryToUnlockThisSkill()
        {
            SkillSlot skillSlot = GetComponent<SkillSlot>();
            if (skillSlot.CanBeUnlocked())
            {
                skillSlot.SetIsUnlocked(true);
                GetComponentInParent<CraftingSystem>().UnlockSkill(skillSlot);
            }
        }
    }
}

