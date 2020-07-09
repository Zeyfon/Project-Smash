using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallexEffect : MonoBehaviour
{
    [SerializeField] Transform cameraTransform =null;
    Vector2 startPosition;

    [SerializeField] List<ParallexEffectComponents> tests;



    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach(ParallexEffectComponents parallexs in tests)
        {
            float newPositionx = startPosition.x + ((cameraTransform.position.x-startPosition.x) * parallexs.factorX);
            float newPositiony = startPosition.y + ((cameraTransform.position.y-startPosition.y) * parallexs.factorY);
            parallexs.transform.position = new Vector2(newPositionx, newPositiony);
        }
    }
}
