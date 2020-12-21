using UnityEditor;
using UnityEngine;

namespace PSmash.Attributes
{
    public class EventManager : MonoBehaviour
    {
        public delegate void PlayerDamaged(float healthScale);
        public static event PlayerDamaged PlayerIsDamaged;

        public delegate void ActiveItemChanged(int item);
        public static event ActiveItemChanged ItemChanged;

        public delegate void GamePaused();
        public static event GamePaused PauseGame;

        public delegate void GameUnpaused();
        public static event GameUnpaused UnpauseGame;

        public delegate void StartButtonAction();
        public static event StartButtonAction StartButtonPressed;

        public delegate void BootsObtained();
        public static event BootsObtained PlayerGotBoots;

        public delegate void SubWeaponChange();
        public static event SubWeaponChange SubWeaponchangeDone;

        public delegate void PlayerPerformsUncontrolledAction(bool state);
        public static event PlayerPerformsUncontrolledAction PlayerPerformUncontrolledAction;

        public void PlayerReceivedDamage(float healthScale)
        {
            if (PlayerIsDamaged == null) return;
            PlayerIsDamaged(healthScale);
        }

        public void PlayerChangeActiveItem(int item)
        {
            print("ItemChangeEventProduced");
            if (ItemChanged == null) return;
            ItemChanged(item);
        }


        public void PressingPauseButton()
        {
            StartButtonPressed();
        }
        public void GameWillPause()
        {
            if (PauseGame == null) return;
            PauseGame();
        }

        public void GameWillUnpause()
        {
            if (UnpauseGame == null) return;
            UnpauseGame();
        }

        public void PlayerGotTheBoots()
        {
            Debug.Log("Player Got the Boots");
            if (PlayerGotBoots == null) return;
            PlayerGotBoots();
        }

        public void SubWeabonChange()
        {
            if (SubWeaponchangeDone == null) return;
            SubWeaponchangeDone();
        }

        public void PlayerControlDisable()
        {
            if (PlayerPerformUncontrolledAction == null) return;
            PlayerPerformUncontrolledAction(false);
        }
        public void PlayerControlEnable()
        {
            if (PlayerPerformUncontrolledAction == null) return;
            PlayerPerformUncontrolledAction(true);
        }
    }
}

