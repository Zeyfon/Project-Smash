using UnityEngine;


namespace PSmash.SceneManagement
{
    [System.Serializable]
    public class ParallexEffectComponents
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

