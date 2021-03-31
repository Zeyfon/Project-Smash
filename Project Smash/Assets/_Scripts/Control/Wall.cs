using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Attributes;
using PSmash.Movement;

namespace PSmash.Control
{

    public class Wall : MonoBehaviour
    {

        bool isPlayerOnWall = false;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                //print("Player inside Wall");
                //collision.GetComponent<PlayerMovement>().IsWallDetected(true);
                //SecondaryWeaponSystem weapons = collision.GetComponent<SecondaryWeaponSystem>();
                //if (weapons != null) weapons.CanMoveOnWall(true);
                //ToolSystem tools = collision.GetComponent<ToolSystem>();
                //if (tools != null) tools.WallDetected(true);
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                //print("Player outside wall");
                //collision.GetComponent<PlayerMovement>().IsWallDetected(false);

                // SecondaryWeaponSystem weapons = collision.GetComponent<SecondaryWeaponSystem>();
                //if (weapons != null) weapons.CanMoveOnWall(false);
                //ToolSystem tools = collision.GetComponent<ToolSystem>();
                //if (tools != null) tools.WallDetected(false);
            }
        }

        public bool IsPlayerOnWall()
        {
            return isPlayerOnWall;
        }
    }
}

