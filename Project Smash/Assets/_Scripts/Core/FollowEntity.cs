using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Movement
{
    public class FollowEntity : MonoBehaviour
    {

        [SerializeField] bool canMove = true;

        Transform target;
        Vector2 offset;

        private void Start()
        {
            EnemyMovement movement = transform.parent.GetComponentInChildren<EnemyMovement>();
            if(movement != null)
            {
                target = movement.transform;
                offset = transform.position - target.position;
            }
        }


        void Update()
        {
            if (!canMove) return;
            transform.position = new Vector2(target.position.x + offset.x, target.position.y + offset.y);
        }

        public void SetData(Transform target, bool canMove) 
        {
            this.target = target;
            this.canMove = canMove;
        }
    }
}

