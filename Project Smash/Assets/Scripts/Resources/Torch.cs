using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Torch : MonoBehaviour
{
    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Interactable"))
        {
            print(collision.gameObject);
            if (collision.transform.GetComponentInChildren<Light2D>())
            {
                print("Turning on Lights");
                transform.GetChild(0).gameObject.SetActive(true);
                GetComponentInChildren<ParticleSystem>().Play();
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}
