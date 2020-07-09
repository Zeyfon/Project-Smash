using PSmash.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardBar : MonoBehaviour
{
    [SerializeField] float timerThreshold = 3;
    [Tooltip("Time it will take to regen the whole bar")]
    [SerializeField] float regenTime = 1;
    Coroutine coroutine;
    EnemyHealth health;

    bool crRunning = false;
    private void Start()
    {
        health = transform.parent.GetComponentInChildren<EnemyHealth>();
    }
    //Its purpose is for identification with the GameObject.FindObjectOfType<GuardBar>();
    public void SubstractDamage(float valueScale, int initialGuardValue)
    {
        transform.localScale = new Vector2(valueScale, transform.localScale.y);
        if(coroutine != null)
        {
            StopCoroutine(coroutine);
        }
        coroutine = StartCoroutine(RegenGuard(valueScale, initialGuardValue));
    }

    IEnumerator RegenGuard(float valueScale, int initialGuardValue)
    {
        crRunning = true;
        float timer = 0;
        //print("Timer to regen Guards starts");
        while (timer < timerThreshold)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //print("Regen Guards starts");
        float guardValue = transform.localScale.x;
        while (guardValue < 1)
        {
            guardValue += Time.deltaTime / regenTime;
            if (guardValue > 1) guardValue = 1;
            transform.localScale = new Vector2(guardValue, transform.localScale.y);
            health.RegeneratingGuard(guardValue);
            yield return new WaitForEndOfFrame();
        }
        //print("Regen Guard finished");
        crRunning = false;
    }
}
