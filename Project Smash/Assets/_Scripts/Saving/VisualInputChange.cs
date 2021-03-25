using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.InputSystem;
using UnityEngine.InputSystem;
using System;

namespace PSmash.Saving
{

    public class VisualInputChange : MonoBehaviour, IShowInputPrompt, IHideInputPrompt
    {
        [SerializeField] SpriteRenderer controlSprite = null;
        [SerializeField] SpriteRenderer keyboardSprite = null;
        [SerializeField] float fadeTime = 0.5f;
        PlayerInput playerInput;

        void Awake()
        {
            controlSprite.color = new Color(1, 1, 1, 0);
            keyboardSprite.color = new Color(1, 1, 1, 0);
            Transform controller = GameObject.FindGameObjectWithTag("Player").transform.GetChild(0);
            playerInput = controller.GetComponent<PlayerInput>();
            CheckControlInput(playerInput);
        }

        private void OnEnable()
        {
            playerInput.onControlsChanged += Input_onControlsChanged;
        }

        private void OnDisable()
        {
            playerInput.onControlsChanged -= Input_onControlsChanged;
        }

        private void Input_onControlsChanged(PlayerInput obj)
        {
            //print("Input Scheme Changed");
            CheckControlInput(obj);
        }

        private void CheckControlInput(PlayerInput obj)
        {
            if (obj.currentControlScheme == "Gamepad")
            {
                //print("Change to Gamepad");
                ShowGamepadPrompt();
            }
            else
            {
                //print("Change to Keyboard");
                ShowKeyboardPrompt();
            }
        }
        void ShowGamepadPrompt()
        {
            controlSprite.enabled = true;
            keyboardSprite.enabled = false;
        }

        void ShowKeyboardPrompt()
        {
            controlSprite.enabled = false;
            keyboardSprite.enabled = true;
        }



        public void HideInputPrompt()
        {
            HidePrompt();
        }

        void HidePrompt()
        {
            StartCoroutine(HideFinisherPrompt(controlSprite));
            StartCoroutine(HideFinisherPrompt(keyboardSprite));
        }

        public void ShowInputPrompt()
        {
            ShowPrompt();
        }

        void ShowPrompt()
        {
            StartCoroutine(ShowFinisherPrompt(controlSprite));
            StartCoroutine(ShowFinisherPrompt(keyboardSprite));
        }
        private IEnumerator HideFinisherPrompt(SpriteRenderer spriteRenderer)
        {
            yield return FadeOut(spriteRenderer, fadeTime);
        }
        private IEnumerator ShowFinisherPrompt(SpriteRenderer spriteRenderer)
        {
            yield return FadeIn(spriteRenderer, fadeTime);
        }

        Coroutine FadeOut(SpriteRenderer spriteRenderer, float time)
        {
            return Fade(spriteRenderer, 0, time);
        }

        Coroutine FadeIn(SpriteRenderer spriteRenderer, float time)
        {
            return Fade(spriteRenderer, 1, time);
        }

        Coroutine Fade(SpriteRenderer spriteRenderer, float target, float time)
        {
            Coroutine coroutine = StartCoroutine(Fading(spriteRenderer, target, time));
            return coroutine;
        }

        IEnumerator Fading(SpriteRenderer spriteRenderer, float target, float time)
        {
            float alpha = spriteRenderer.color.a;
            while (!Mathf.Approximately(alpha, target))
            {
                alpha = Mathf.MoveTowards(alpha, target, Time.deltaTime / time);
                spriteRenderer.color = new Color(1, 1, 1, alpha);
                yield return null;
            }
        }
    }

}
