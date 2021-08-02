using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Combat
{
    public class GrapingHookTarget : MonoBehaviour
    {
        [SerializeField] int level = 1;
        [SerializeField] Sprite level1Sprite = null;
        [SerializeField] Sprite level2Sprite = null;
        [SerializeField] SpriteRenderer promptSpriteRenderer = null;
        [SerializeField] SpriteRenderer levelSpriteRenderer = null;

        [UnityEngine.Tooltip("As it decreases, it enables more angle aperture to use the grapin hook. 0 = can grap from anywhere as long as you are in range ")]
        [Range(0, 1)]
        [SerializeField] float targetApertureRange = 0.25f;


        private void Awake()
        {
            promptSpriteRenderer.enabled = false;
            if (level == 1)
                levelSpriteRenderer.sprite = level1Sprite;
            else
                levelSpriteRenderer.sprite = level2Sprite;
        }

        public void EnablePrompt()
        {
            promptSpriteRenderer.enabled = true;
        }

        public void DisablePrompt()
        {
            promptSpriteRenderer.enabled = false;
        }

        public int GetMyLevel()
        {
            return level;
        }
    }

}
