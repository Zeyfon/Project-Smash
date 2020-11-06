using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace PSmash.Resources
{
    public class CarryOnFireFlies : MonoBehaviour
    {
        [SerializeField] float timeToFadeOut = 10;
        Light2D pointLight;
        float normalizetimer;

        float initialInteranlRadius;
        float initialOuterRadius;
        // Start is called before the first frame update
        void Start()
        {
            normalizetimer = 1;
            pointLight = GetComponentInChildren<Light2D>();
            initialInteranlRadius = pointLight.pointLightInnerRadius;
            initialOuterRadius = pointLight.pointLightOuterRadius;
        }

        // Update is called once per frame
        void Update()
        {
            float multiplier = 1;
            if (normalizetimer < 0.4f) multiplier = 2.5f;
            normalizetimer -= Time.deltaTime / (timeToFadeOut * multiplier);
            if (normalizetimer <= 0) Destroy(gameObject);
            else
            {
                pointLight.pointLightInnerRadius = initialInteranlRadius * normalizetimer;
                pointLight.pointLightOuterRadius = initialOuterRadius * normalizetimer;
            }
        }
    }
}

