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
            GetComponentInParent<CraftingSystem>().TryToUnlockSkill(this);
        }
    }
}

