using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDummy : MonoBehaviour, IGrapingHook, IWeight
{
    
    [SerializeField] bool isHeavy = true;
    bool isGrapingHookFinished = false;
    public enum Weight
    {
        low,
        heavy
    }

    [SerializeField] Weight weight;


    //////////////////////////////////////////////////////////////////////////////PUBLIC///////
    public void Hooked()
    {
        isGrapingHookFinished = false;
        if (weight == Weight.low)
        {
            //TODO 
            //MOVE TO THE FRONT OF THE PLAYER
            //StartCoroutine(MoveToTheFrontOfThePlayer(attackerTransform));
            //STAGGER
            isHeavy = false;
        }

        else
        {
            //TODO 
            //MOVE THE PLAYER TO THE FRONT OF YOU
            //STAGGER
            isHeavy = true;
        }
    }

    public bool IsHeavy()
    {
        return isHeavy;
    }

    public bool GetISGrapingHookFinished()
    {
        return isGrapingHookFinished;
    }

    ////////////////////////////////////////////////////////////////////////////PRIVATE\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    
    IEnumerator MoveToTheFrontOfThePlayer(Transform playerTransform)
    {
        float speed = 10;
        float y = transform.position.y + 0.3f;
        Movement(playerTransform, speed, y);
        while (Vector3.Distance(playerTransform.position,transform.position)>1)
        {
            yield return new WaitForFixedUpdate();
            Movement(playerTransform, speed, y);
        }
        isGrapingHookFinished = true;
        Stagger();
    }

    void Movement(Transform playerTransform, float speed, float y)
    {
        float x = transform.position.x + (speed * playerTransform.right.x * -1 * Time.fixedDeltaTime);
        transform.position = new Vector3(x, y, transform.position.z);
    }

    void Stagger()
    {
        print("Stagger");
    }

    public bool IWeight()
    {
        if (weight == Weight.low)
            return false;
        else
            return true;
    }

    public void Pulled()
    {
        throw new NotImplementedException();
    }
}
