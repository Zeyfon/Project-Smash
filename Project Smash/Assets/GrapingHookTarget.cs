using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapingHookTarget : MonoBehaviour
{
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [Tooltip("As it decreases, it enables more angle aperture to use the grapin hook. 0 = can grap from anywhere as long as you are in range ")]
    [Range(0,1)]
    [SerializeField] float targetApertureRange = 0.25f;
    static Transform targetTransform = null;
    static float distance = 0;
    static int count = 0;
    private void LateUpdate()
    {
        if(targetTransform == transform)
        {
            spriteRenderer.enabled = true;
        }
        else
        {
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            count++;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        //print("Staying");
        if (collision.CompareTag("Player"))
            CheckToEnablePrompt(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
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

    void CheckToEnablePrompt(Collider2D collision)
    {

        if (IsPlayerLookingAtMe(collision.transform) && IsOurDisttanceTheLongest(collision.transform) && IsAbovePlayer(collision.transform))
        {
            targetTransform = transform;
        }
        else if(!IsPlayerLookingAtMe(collision.transform) && targetTransform==transform)
        {
            targetTransform = null;
        }
    }

    private bool IsAbovePlayer(Transform playerTransform)
    {
        if (playerTransform.position.y < transform.position.y)
            return true;
        else
            return false;
    }

    private bool IsOurDisttanceTheLongest(Transform playerTransform)
    {
        if (Vector2.Distance(playerTransform.position, transform.position) > distance)
            return true;
        else
            return false;
    }

    private bool IsPlayerLookingAtMe(Transform playerTransform)
    {
        Vector3 direction = (transform.position - playerTransform.position).normalized;
        float dotProduct = Vector3.Dot(direction, playerTransform.right);
        print(dotProduct);
        if (dotProduct > 0.25f)
            return true;
        else
            return false;
    }

}
