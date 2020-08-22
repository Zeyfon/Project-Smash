using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public static event Action StartObtained;
    [SerializeField] GameObject starEffectGameObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartObtained();
            Instantiate(starEffectGameObject, transform.position, Quaternion.Euler(0,0,90));

            Destroy(gameObject);
        }
    }

}
