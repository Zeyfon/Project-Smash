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
    static Transform targetTransform = null;
    static float distance = 0;
    static int count = 0;

    static FsmObject currentFSM;

    private void Awake()
    {
        currentFSM = FsmVariables.GlobalVariables.FindFsmObject("currentFSM");
        spriteRenderer.enabled = false;
    }

    private void Update()
    {
        if ((currentFSM.Value as PlayMakerFSM).FsmName == "SubweaponState")
        {
            if (targetTransform != transform)
                spriteRenderer.enabled = false;
        }
    }

    public static Transform TargetTransform
    {
        get
        {
            return targetTransform;
        }
    }




    void CheckToEnablePrompt(Collider2D collision)
    {
        if ((currentFSM.Value as PlayMakerFSM).FsmName == "SubweaponState")
        {
            return;
        }
        if (IsPlayerLookingAtMe(collision.transform) && IsOurDisttanceTheLongest(collision.transform) && IsAbovePlayer(collision.transform))
        {
            targetTransform = transform;
            spriteRenderer.enabled = true;
            return;
        }

        if(!IsPlayerLookingAtMe(collision.transform) && targetTransform==transform)
        {
            targetTransform = null;

        }
        spriteRenderer.enabled = false;
    }

    bool IsAbovePlayer(Transform playerTransform)
    {
        if (playerTransform.position.y < transform.position.y)
            return true;
        else
            return false;
    }

    bool IsOurDisttanceTheLongest(Transform playerTransform)
    {
        if (Vector2.Distance(playerTransform.position, transform.position) > distance)
            return true;
        else
            return false;
    }

    bool IsPlayerLookingAtMe(Transform playerTransform)
    {
        Vector3 direction = (transform.position - playerTransform.position).normalized;
        float dotProduct = Vector3.Dot(direction, playerTransform.right);
        if (dotProduct > 0.25f)
            return true;
        else
            return false;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            count++;
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            CheckToEnablePrompt(collision);
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            count--;
            if (count <= 0)
            {
                targetTransform = null;
            }
        }
    }
}
