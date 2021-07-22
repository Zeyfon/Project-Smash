using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookRope : MonoBehaviour
{

    [SerializeField] Transform targetTransform;
    [SerializeField] LineRenderer line;
    [SerializeField] float timeToReachTarget = 0.15f;
    // Start is called before the first frame update
    Vector2 velocity;
    Vector2 headPosition;
    float timer = 0;
    public IEnumerator DoHookShot(List<Transform> points)
    {
        Setup(points);
        yield return MoveRopeHeadToTarget();
        timer = 0;
    }

    public void PlayerPulled()
    {
        StartCoroutine(MoveRopeBaseToTarget());
    }
    public void EnemyPulled(Transform enemyTransform)
    {
        StartCoroutine(MoveHeadWithEnemy(enemyTransform));
    }

    IEnumerator MoveHeadWithEnemy(Transform enemyTransform)
    {
        line.SetPosition(0, transform.position);
        float distance = Mathf.Infinity;
        while (distance > 1)
        {
            distance = Vector3.Distance(enemyTransform.position, transform.position);
            line.SetPosition(1, enemyTransform.position);
            //timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        line.SetPosition(1, transform.position);
        Destroy(gameObject, 0.1f);
    }

    void Setup(List<Transform> points)
    {
        transform.position = points[0].position;
        targetTransform = points[1];
        line.SetPosition(0, transform.position);
        Vector2 distance = targetTransform.position - transform.position;
        velocity = distance / timeToReachTarget;
        headPosition = transform.position;
    }

    IEnumerator MoveRopeHeadToTarget()
    {
        while(timer < timeToReachTarget)
        {
            Vector2 movement = velocity * Time.fixedDeltaTime;
            headPosition += movement;
            line.SetPosition(0, transform.position);
            line.SetPosition(1, headPosition);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        line.SetPosition(1, targetTransform.position);
    }

    IEnumerator MoveRopeBaseToTarget()
    {
        line.SetPosition(0, transform.position);
        float distance = Mathf.Infinity;
        while (distance > 3 && timer < timeToReachTarget)
        {
            distance = Vector3.Distance(targetTransform.position, transform.position);
            //MoveBase();
            line.SetPosition(0, transform.position);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        line.SetPosition(0, targetTransform.position);
        Destroy(gameObject, 0.1f);
    }
}
