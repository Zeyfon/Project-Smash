using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.CraftingSystem;

namespace PSmash.UI.CraftingSytem
{
    /// <summary>
    /// Only use is the OnClick UnityEvent to try to unlock this skill in the CraftingSystem
    /// </summary>
    public class SkillSlotButton : MonoBehaviour
    {
        public void TryToUnlockThisSkill()
        {
            GetComponentInParent<CraftingSystem.CraftingSystem>().TryToUnlockSkill(GetComponent<SkillSlot>());
        }
    }
}

