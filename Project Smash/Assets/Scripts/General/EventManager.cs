using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void PlayerDamaged( float healthScale);
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
}
