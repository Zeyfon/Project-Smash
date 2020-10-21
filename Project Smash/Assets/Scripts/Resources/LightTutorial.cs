﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PSmash.Control;
using PSmash.Movement;

namespace PSmash.Resources
{
    public class LightTutorial : MonoBehaviour
    {
        [SerializeField] float fadeTime = 0.5f;
        [SerializeField] Text text;

        bool cr_Running = false;
        bool canTipsRun = true;

        PlayerMovementV2 playerMovement;
        // Start is called before the first frame update
        void Awake()
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, 0);
        }

        private void OnEnable()
        {
            if (playerMovement == null) return;
            playerMovement.OnPlayerWallState += IsPlayerMovingOnWall;
        }
        private void OnDisable()
        {
            if (playerMovement == null) return;
            playerMovement.OnPlayerWallState -= IsPlayerMovingOnWall;
        }

        void IsPlayerMovingOnWall(bool isPlayerOnWall)
        {
            if (isPlayerOnWall)
            {
                DisableTip();
            }
            else
            {
                EnableTips();
            }
        }
        
        void DisableTip()
        {
            print("Disabling Tips");
            canTipsRun = false;
            if (text.color.a == 0) return;
            HideTips();
        }
        void EnableTips()
        {
            print("Enabling Tips");
            canTipsRun = true;
        }

        private void HideTips()
        {
            if (cr_Running) StopAllCoroutines();//StopCoroutine(currentCoroutine);
            StartCoroutine(HideHelp());
        }

        IEnumerator ShowHelp()
        {
            cr_Running = true;
            print("Start Showing");
            float alpha = 0;
            while(alpha != 1)
            {
                alpha += Time.deltaTime/ fadeTime;
                if (alpha > 1) alpha = 1;
                text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
                yield return null;
            }
            print("End Showing");
            cr_Running = false;
        }

        IEnumerator HideHelp()
        {
            cr_Running = true;
            print("Start Hiding");
            float alpha = 1;
            while (alpha != 0)
            {
                alpha -= Time.deltaTime / fadeTime;
                if (alpha < 0) alpha = 0;
                text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);
                yield return null;
            }
            print("End Hiding");
            cr_Running = false;
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (canTipsRun && collision.CompareTag("Player"))
            {
                if (playerMovement == null)
                {
                    playerMovement = collision.GetComponent<PlayerMovementV2>();
                    playerMovement.OnPlayerWallState += IsPlayerMovingOnWall;
                }
                if (cr_Running) StopAllCoroutines();//StopCoroutine(currentCoroutine);
                StartCoroutine(ShowHelp());
            }
        }
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (canTipsRun && collision.CompareTag("Player"))
            {
                HideTips();
            }
        }
    }

}
