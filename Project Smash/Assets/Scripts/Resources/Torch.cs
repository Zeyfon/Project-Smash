using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace PSmash.Resources
{
    public class Torch : MonoBehaviour
    {
        [SerializeField] bool startLigthened = false;
        private void Awake()
        {
            if (!startLigthened) EnableLight(false);
            else EnableLight(true);
        }
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Interactable"))
            {
                print(collision.gameObject);
                Light2D light = collision.transform.GetComponentInChildren<Light2D>();
                if (collision.transform.GetComponentInChildren<Light2D>())
                {
                    print("Turning on Lights");
                    EnableLight(true);
                    GetComponent<Collider2D>().enabled = false;
                }
            }
        }

        private void EnableLight(bool state)
        {
            transform.GetChild(0).gameObject.SetActive(state);
            if(state) GetComponentInChildren<ParticleSystem>().Play();
        }
    }
}

