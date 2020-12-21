using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.SceneManagement
{
    public class BackgroundParallexEffect : MonoBehaviour
    {
        [SerializeField] Transform cameraTransform = null;
        Vector2 startPosition;

        [SerializeField] List<BackgroundLayers> backgroundLayers = null;



        // Start is called before the first frame update
        void Start()
        {
            startPosition = transform.position;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            foreach (BackgroundLayers backgroundLayer in backgroundLayers)
            {
                float newPositionx = startPosition.x + ((cameraTransform.position.x - startPosition.x) * backgroundLayer.factorX);
                float newPositiony = startPosition.y + ((cameraTransform.position.y - startPosition.y) * backgroundLayer.factorY);
                backgroundLayer.transform.position = new Vector2(newPositionx, newPositiony);
            }
        }
    }
}


