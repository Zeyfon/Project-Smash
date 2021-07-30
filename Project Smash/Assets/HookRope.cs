using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookRope : MonoBehaviour
{

    [SerializeField] Transform hookHead = null;
    [SerializeField] LineRenderer line;
    [SerializeField] float timeToReachTarget = 0.15f;
    [SerializeField] float maxDistance = 10;

    Vector2 velocity;
    Vector2 finalPosition;
    Transform enemyTransform;

    public Transform EnemyHitByHook()
    {
        return enemyTransform;
    }

    public void PlayerPulled()
    {
        StartCoroutine(MoveRopeBaseToTarget());
    }

    public void SetRopeToFollowEnemyDisplacement(Transform enemyTransform)
    {
        StartCoroutine(MoveHeadWithEnemy(enemyTransform));
    }

    public IEnumerator DoHookShot(Transform target)
    {
        float timer = 0;
        Setup(target);
        Vector2 headPosition = transform.position;
        while (timer < timeToReachTarget)
        {
            if (enemyTransform != null && target == null)
                yield break;
            Vector2 movement = velocity * Time.fixedDeltaTime;
            headPosition += movement;
            hookHead.position = headPosition;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, headPosition);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    void Setup(Transform target)
    {
        if (target)
        {
            finalPosition = target.position;
        }
        else
        {
            finalPosition = transform.position + transform.parent.right * maxDistance;
        }
        line.SetPosition(0, transform.position);
        Vector2 distance = finalPosition - (Vector2)transform.position;
        velocity = distance / timeToReachTarget;
    }


    IEnumerator MoveRopeBaseToTarget()
    {
        float timer = 0;
        line.SetPosition(0, transform.position);
        float distance = Mathf.Infinity;
        while (distance > 2.5f && timer < timeToReachTarget)
        {
            line.SetPosition(0, transform.position);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator MoveHeadWithEnemy(Transform enemyTransform)
    {
        line.SetPosition(0, transform.position);
        float distance = Mathf.Infinity;
        while (distance > 1)
        {
            distance = Vector3.Distance(enemyTransform.position, transform.position);
            line.SetPosition(1, enemyTransform.position);
            yield return new WaitForFixedUpdate();
        }
        line.SetPosition(1, transform.position);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            enemyTransform = collision.transform;
        }
    }
}
