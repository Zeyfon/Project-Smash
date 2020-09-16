using PSmash.Core;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PSmash.UI
{
    public class SecondaryWeaponsSelector : MonoBehaviour
    {
        [SerializeField] Image weaponEquippedImage;
        [SerializeField] Image weaponOnHoldImage;

        Sprite spriteEquipped;
        Sprite spriteOnHold;

        bool isFinished = false;
        bool isChangingWeapon = false;

        void OnEnable()
        {
            EventManager.SubWeaponchangeDone += ChangeSubWeapon;
        }

        private void OnDisable()
        {
            EventManager.SubWeaponchangeDone -= ChangeSubWeapon;

        }

        private void ChangeSubWeapon()
        {
            isFinished = false;
            if (isChangingWeapon)
            {
                weaponEquippedImage.sprite = spriteOnHold;
                weaponOnHoldImage.sprite = spriteEquipped;
                isChangingWeapon = false;
            }
            StartCoroutine(ChangeWeapon());
        }

        IEnumerator ChangeWeapon()
        {
            isChangingWeapon = true;
            spriteEquipped = weaponEquippedImage.sprite;
            spriteOnHold = weaponOnHoldImage.sprite;
            GetComponent<Animator>().Play("WeaponChange");
            while (!isFinished)
            {
                yield return new WaitForEndOfFrame();
            }
            weaponEquippedImage.enabled = false;
            weaponOnHoldImage.enabled = false;
            weaponEquippedImage.sprite = spriteOnHold;
            weaponOnHoldImage.sprite = spriteEquipped;
            weaponEquippedImage.enabled = true;
            weaponOnHoldImage.enabled = true;
            isChangingWeapon = false;
        }
        void Finished()
        {
            isFinished = true;
        }
    }
}

