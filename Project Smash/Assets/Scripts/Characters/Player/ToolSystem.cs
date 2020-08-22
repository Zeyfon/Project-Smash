﻿using PSmash.Combat;
using PSmash.Control;
using PSmash.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class ToolSystem : MonoBehaviour
{
    [SerializeField] SecondaryWeaponsList toolEquipped;

    [SerializeField] Sprite[] secondaryWeaponSprites;
    ToolActionList toolActions;
    PlayerMovementV2 movement;
    PlayerControllerV2 playerController;
    EventManager eventManager;
    bool wallDetected = false;
    bool hikingSpykeUnlocked = true;
    // Start is called before the first frame update
    void Awake()
    {
        movement = GetComponent<PlayerMovementV2>();
        eventManager = FindObjectOfType<EventManager>();
        playerController = GetComponent<PlayerControllerV2>();
    }

    public void ChangeActiveWeapon()
    {
        //Debug.Log("Changing Active Secondary Weapon");
        switch (toolEquipped)
        {

            case SecondaryWeaponsList.Bomb:
                toolEquipped = SecondaryWeaponsList.Spike;
                eventManager.SubWeabonChange();
                break;
            case SecondaryWeaponsList.Spike:
                toolEquipped = SecondaryWeaponsList.Bomb;
                eventManager.SubWeabonChange();
                break;
        }
        AnalyticsEvent.Custom("WeaponChange", new Dictionary<string, object>
        {
            {"weapon",toolEquipped }
        });
    }

    public void WallDetected(bool state)
    {
        wallDetected = state;
        if (state == true)
        {
            //print("Inside Wall");
            playerController.CurrentToolAction(ToolActionList.WallMovement);
        }
        else 
        {
            //print("Outside wall");
            playerController.CurrentToolAction(ToolActionList.NoAction);
        }
        if(movement.IsMovingOnWall && !wallDetected)
        {
            playerController.StopWallMovement();
        }
    }
}
