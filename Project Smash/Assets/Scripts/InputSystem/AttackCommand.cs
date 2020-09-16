using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Control;

namespace PSmash.InputSystem
{
    public class AttackCommand : ActionCommand, ICommand
    {
        bool isButtonPressed = false;
        bool holdAttack = false;
        [SerializeField] float holdTime = 0.5f;

        Coroutine coroutine;

        void ICommand.Execute(PlayerController playerController)
        {
            if (holdAttack)
            {
                isButtonPressed = false;
                holdAttack = false;
                return;
            }
            isButtonPressed = !isButtonPressed;
            if (isButtonPressed)
            {
                coroutine = StartCoroutine(IsButtonHold(playerController));
                //Debug.Log("Coroutine Started");
            }
            else
            {
                isButtonPressed = false;
                //Debug.Log("Pressed Attack");
                StopCoroutine(coroutine);
                Attack(playerController, true);
            }
        }

        IEnumerator IsButtonHold(PlayerController playerController)
        {
            float timer = 0;
            while (timer < holdTime)
            {
                timer += Time.deltaTime;
                //print("Holding");
                yield return new WaitForEndOfFrame();
            }
            //Debug.Log("Hold Attack");
            Attack(playerController, false);

            holdAttack = true;
        }
        void Attack(PlayerController playerController, bool isPressedAttack)
        {
            playerController.AttackButtonPressed(isPressedAttack);
        }
    }
}

