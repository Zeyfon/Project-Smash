using UnityEngine;

namespace PSmash.Combat
{
    public class GrapingHookTargetDetector : MonoBehaviour
    {
        [SerializeField] float radiusDetection = 10;
        [SerializeField] LayerMask whatIsATargetHook;
        [SerializeField] float targetApertureRange = 0.25f;

        Transform hookTarget;
        float distance = 0;

        void FixedUpdate()
        {
            Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, radiusDetection + 3, whatIsATargetHook);
            if (targets.Length > 0)
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
            foreach (Collider2D coll in targets)
            {
                if (IsTargetInFront(coll.transform) && IsAbovePlayer(coll.transform))
                {
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

