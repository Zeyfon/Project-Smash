using PSmash.Inventories;
using UnityEngine;
using UnityEngine.UI;

namespace PSmash.UI
{
    public class SubweaponUI : MonoBehaviour
    {
        [SerializeField] Image image = null;

        private void Awake()
        {
            image.enabled = false;
        }

        private void OnEnable()
        {
            Equipment.onSubWeaponChange += UpdateSubWeaponDisplay;
        }

        private void OnDisable()
        {
            Equipment.onSubWeaponChange -= UpdateSubWeaponDisplay;
        }

        private void UpdateSubWeaponDisplay(Weapon subWeapon)
        {
            if (subWeapon == null)
                return;
            else
            {
                image.sprite = subWeapon.GetSprite();
                image.enabled = true;
            }
        }
    }
}

