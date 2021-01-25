using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.UI.DamageText
{
    public class SpawnDamageText : MonoBehaviour
    {
        [SerializeField] DamageText damageTextPrefab;
        public void Spawn(float damage)
        {
            DamageText textPrefab = Instantiate<DamageText>(damageTextPrefab, transform);
            textPrefab.SetDamagaeValue(damage);
        }
    }
}
