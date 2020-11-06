using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Core
{
    public class FollowEntity : MonoBehaviour
    {
        public Transform target;
        [SerializeField] Vector2 offset;
        [SerializeField] bool canMove = false;

        // Update is called once per frame
        void Update()
        {
            //print(target);
            if (!canMove) return;
            //print(gameObject.name + " Moving");
            transform.position = new Vector2(target.position.x + offset.x, target.position.y + offset.y);
        }

        public void SetData(Transform target, bool canMove) 
        {

            this.target = target;
            this.canMove = canMove;
            //print(target + "  " + canMove);
        }
    }
}

