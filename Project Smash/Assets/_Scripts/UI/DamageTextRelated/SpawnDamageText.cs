using PSmash.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.UI.DamageText
{
    public class SpawnDamageText : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPrefab;
        [SerializeField] float offset = 0.6f;

        Vector3 vOffset;
        Color color;
        float sizeFactor = 1;
        public void Spawn(Health.DamageSlot slot)
        {
            if(slot.damageType == Health.DamageType.Posture)
            {
                vOffset = new Vector3(-offset, 0, 0);
                color = Color.blue;
            }
            else
            {
                vOffset = new Vector3(offset, 0, 0);
                color = Color.red;
            }

            if (slot.criticalType == Health.CriticalType.Critical)
            {
                color = Color.yellow;
                sizeFactor = 1.5f;
            }
            DamageText textPrefab = Instantiate<DamageText>(damageTextPrefab, transform.position + vOffset, Quaternion.identity, transform);
            textPrefab.Setup(slot.damage, color, sizeFactor);
        }
    }
}
