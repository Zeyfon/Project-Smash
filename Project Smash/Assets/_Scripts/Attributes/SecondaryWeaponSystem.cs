using PSmash.Combat;
using PSmash.Movement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

namespace PSmash.Attributes
{
    public class SecondaryWeaponSystem : MonoBehaviour
    {
        [SerializeField] SecondaryWeaponsList secondaryWeaponEquipped;
        [SerializeField] Sprite[] secondaryWeaponSprites;
        PlayerFighter fighter;
        PlayerMovement movement;
        EventManager eventManager;
        bool canMoveOnWall = false;
        // Start is called before the first frame update
        void Start()
        {
            fighter = GetComponent<PlayerFighter>();
            movement = GetComponent<PlayerMovement>();
            eventManager = FindObjectOfType<EventManager>();
        }

        public void ChangeActiveWeapon()
        {
            //Debug.Log("Changing Active Secondary Weapon");
            switch (secondaryWeaponEquipped)
            {

                case SecondaryWeaponsList.Bomb:
                    secondaryWeaponEquipped = SecondaryWeaponsList.Spike;
                    eventManager.SubWeabonChange();
                    break;
                case SecondaryWeaponsList.Spike:
                    secondaryWeaponEquipped = SecondaryWeaponsList.Bomb;
                    eventManager.SubWeabonChange();
                    break;
            }
            AnalyticsEvent.Custom("WeaponChange", new Dictionary<string, object>
        {
            {"weapon",secondaryWeaponEquipped }
        });
        }

        public void PerformSecondaryWeaponAction(IIsAttacking action)
        {
            Debug.Log("Performing Secondary Action");
            if (canMoveOnWall)
            {
                WallMovement();
                return;
            }
            switch (secondaryWeaponEquipped)
            {
                case SecondaryWeaponsList.Bomb:
                    //fighter.SecondaryWeaponAttack(20);
                    action.IIsAttacking();

                    break;
                case SecondaryWeaponsList.Spike:
                    //fighter.SecondaryWeaponAttack(60);
                    action.IIsAttacking();
                    break;
            }
        }

        void WallMovement()
        {
            //movement.SwitchWallMovement();
        }

        public void CanMoveOnWall(bool state)
        {
            canMoveOnWall = state;
            if (movement.IsMovingOnWall && !canMoveOnWall)
            {
                movement.IsMovingOnWall = true;
            }
        }
    }

}
