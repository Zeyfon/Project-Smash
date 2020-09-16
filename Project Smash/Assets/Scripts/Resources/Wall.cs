using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Resources
{
    public class Wall : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                SecondaryWeaponSystem weapons = collision.GetComponent<SecondaryWeaponSystem>();
                if (weapons != null) weapons.CanMoveOnWall(true);
                ToolSystem tools = collision.GetComponent<ToolSystem>();
                if (tools != null) tools.WallDetected(true);

            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                SecondaryWeaponSystem weapons = collision.GetComponent<SecondaryWeaponSystem>();
                if (weapons != null) weapons.CanMoveOnWall(false);
                ToolSystem tools = collision.GetComponent<ToolSystem>();
                if (tools != null) tools.WallDetected(false);
            }
        }
    }
}

