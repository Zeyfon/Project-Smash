using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Core
{
    public class FollowEntity : MonoBehaviour
    {
        Transform target;
        Vector2 offset;
        // Start is called before the first frame update
        void Start()
        {
            target = transform.parent.GetChild(2);
            offset = transform.position - target.position;
            //print(target.position + "  " + offset);
        }

        // Update is called once per frame
        void Update()
        {
            transform.position = new Vector2(target.position.x + offset.x, target.position.y + offset.y);
        }
    }
}

