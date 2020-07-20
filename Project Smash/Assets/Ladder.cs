using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] PlatformEffector2D platform = null;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InvertPlatform()
    {
        platform.rotationalOffset = 180;
        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        yield return new WaitForSeconds(0.3f);
        platform.rotationalOffset = 0;
    }
}
