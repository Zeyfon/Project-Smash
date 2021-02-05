using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.SceneManagement
{
    public class BackgroundParallexEffect : MonoBehaviour
    {
        [SerializeField] Transform origin = null;
        [SerializeField] BackgroundLayers[] backgroundLayers = null;

        Transform cameraTransform;
        private void Awake()
        {
            cameraTransform = Camera.main.transform;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            foreach (BackgroundLayers backgroundLayer in backgroundLayers)
            {
                float newPositionx = origin.position.x + ((cameraTransform.position.x - origin.position.x) * backgroundLayer.factorX);
                float newPositiony = origin.position.y + ((cameraTransform.position.y - origin.position.y) * backgroundLayer.factorY);
                backgroundLayer.transform.position = new Vector2(newPositionx, newPositiony);
            }
        }


        /// <summary>
        /// Here is the struct for each layer to be controlled by the main script
        /// Since it only it is used here there is no purpose for this to be elsewhere
        /// </summary>
        [System.Serializable]
        public class BackgroundLayers
        {
            public Transform transform;
            [Range(-0.5f, 1f)]
            public float factorX;
            [Range(-0.5f, 1)]
            public float factorY;

            public void GenerateParallexEffectValues(Transform transform1, float factorX1, float factorY1)
            {
                transform = transform1;
                factorX = factorX1;
                factorY = factorY1;
            }
        }
    }
}


