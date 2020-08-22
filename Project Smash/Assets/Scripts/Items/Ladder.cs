using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    [SerializeField] PlatformEffector2D platform = null;
    // Start is called before the first frame update
    public void InvertPlatform()
    {
        transform.GetChild(0).GetComponent<LadderTop>().InvertPlatform();
    }
}
