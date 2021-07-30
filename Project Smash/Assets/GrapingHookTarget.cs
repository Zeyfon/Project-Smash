using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapingHookTarget : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [UnityEngine.Tooltip("As it decreases, it enables more angle aperture to use the grapin hook. 0 = can grap from anywhere as long as you are in range ")]    
    [Range(0,1)]
    [SerializeField] float targetApertureRange = 0.25f;

    private void Awake()
    {
        spriteRenderer.enabled = false;
    }

    public void EnablePrompt()
    {
        spriteRenderer.enabled = true;
    }

    public void DisablePrompt()
    {
        spriteRenderer.enabled = false;
    }
}
