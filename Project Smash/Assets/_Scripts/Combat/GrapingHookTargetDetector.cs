using PSmash.Inventories;
using UnityEngine;

namespace PSmash.Combat
{
    public class GrapingHookTargetDetector : MonoBehaviour
    {
        [SerializeField] float radiusDetection = 10;
        [SerializeField] LayerMask whatIsATargetHook;
        [SerializeField] float targetApertureRange = 0.25f;
        [SerializeField] Subweapon grapingHook = null;

        Transform hookTarget;
        Equipment equipment;
        float distance = 0;


        private void Awake()
        {
            equipment = transform.parent.GetComponent<Equipment>();
        }


        void FixedUpdate()
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, radiusDetection + 3, whatIsATargetHook);
            if (targets.Length > 0 && equipment.GetEquippedSubweapon().GetID() == "1720dbad-ef7b-4371-acb1-344715d937d5")
            {
                SetHookTarget(targets);
            }
            else
            {
                hookTarget = null;
            }
        }

        public Transform GetHookTarget()
        {
            return hookTarget;
        }

        void SetHookTarget(Collider2D[] targets)
        {
            distance = 0;
            bool targetUpdated = false;
            foreach (Collider2D coll in targets)
            {
                if (coll.transform == transform)
                    continue;
                if (IsTargetInFront(coll.transform) && IsAbovePlayer(coll.transform))
                {
                    print(equipment.GetEquippedSubweapon().GetMyLevel() + "  " + coll.GetComponent<GrapingHookTarget>().GetMyLevel());
                    if (equipment.GetEquippedSubweapon().GetMyLevel() < coll.GetComponent<GrapingHookTarget>().GetMyLevel())
                    {
                        coll.transform.SendMessage("DisablePrompt");
                        return;
                    }
                    targetUpdated = true;
                    float newMaxDistance = SetFartestTargetDistance(distance, coll.transform);
                    if (newMaxDistance > distance)
                    {
                        hookTarget = coll.transform;
                        hookTarget.SendMessage("EnablePrompt");
                    }
                    else
                    {
                        coll.transform.SendMessage("DisablePrompt");
                    }
                }
                else
                {
                    coll.transform.SendMessage("DisablePrompt");
                }

                if (!targetUpdated)
                {
                    hookTarget = null;
                }
            }
        }

        bool IsAbovePlayer(Transform targetTransform)
        {
            if (transform.parent.position.y < targetTransform.position.y)
                return true;
            else
                return false;
        }

        bool IsTargetInFront(Transform targetTransform)
        {
            Vector3 direction = (targetTransform.position - transform.parent.position).normalized;
            float dotProduct = Vector3.Dot(direction, transform.parent.right);
            if (dotProduct > targetApertureRange)
                return true;
            else
                return false;
        }

        float SetFartestTargetDistance(float distance, Transform target)
        {
            float newDistance = Vector2.Distance(transform.parent.position, target.position);
            if (newDistance < radiusDetection)
                return Vector2.Distance(transform.parent.position, target.position);
            else
                return distance;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, radiusDetection);
        }
    }

}

