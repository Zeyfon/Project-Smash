using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatforms : MonoBehaviour
{
    [SerializeField] float timeToStart = 3f;
    [SerializeField] float speed = 3;
    [SerializeField] Transform line = null;

    Vector3 pos2;
    Vector3 pos1;
    Transform platformTrans;

    // Start is called before the first frame update
    void Start()
    {
        pos1 = transform.GetChild(0).transform.position;
        pos2 = transform.GetChild(1).transform.position;
        line.position = pos1;
        platformTrans = transform.GetChild(2).transform;
        StartCoroutine(GetLineReady());
    }

    IEnumerator GetLineReady()
    {
        float distance = Vector3.Distance(pos2, pos1);
        line.GetComponent<SpriteRenderer>().size = new Vector2(distance, 1);
        //line.localScale = new Vector3(distance,1,1);
        float x = pos2.x - pos1.x;
        float y = pos2.y - pos1.y;
        float angle = Mathf.Atan2(y,x) * 180 / Mathf.PI;
        line.eulerAngles = new Vector3(0, 0, angle);
        yield return null;
    }

    //This is called by the Platform Parent (Platform Manager)
    public void StartMovement()
    {
        StartCoroutine(StartTime());
    }

    IEnumerator StartTime()
    {
        yield return new WaitForSeconds(timeToStart);
        StartCoroutine(Movement());
    }

    IEnumerator Movement()
    {
        float step =0;
        float journeyPercentage = 0;
        float journeyLength = Vector3.Distance(pos2, pos1);
        float currentJourneyLength = 0;
        while (true)
        {
            step = 0;
            currentJourneyLength = 0;
            while (currentJourneyLength < journeyLength)
            {
                step += Time.fixedDeltaTime * speed;
                journeyPercentage = step / journeyLength;
                platformTrans.position = Vector3.Lerp(pos1, pos2, journeyPercentage);
                currentJourneyLength = Vector3.Distance(pos1, platformTrans.position);
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(2);
            step = 0;
            currentJourneyLength = 0;
            while (currentJourneyLength < journeyLength)
            {
                step += Time.fixedDeltaTime * speed;
                journeyPercentage = step / journeyLength;
                platformTrans.position = Vector3.Lerp(pos2, pos1, journeyPercentage);
                currentJourneyLength = Vector3.Distance(pos2, platformTrans.position);
                yield return new WaitForFixedUpdate();
            }
            yield return new WaitForSeconds(2);
        }
    }

    private void OnDrawGizmos()
    {
        pos1 = transform.GetChild(0).transform.position;
        pos2 = transform.GetChild(1).transform.position;
        Gizmos.color = new Color(.8f, .1f, 1, 0.7f);
        Gizmos.DrawWireSphere(pos2, 0.5f);
        Gizmos.DrawWireSphere(pos1, 0.5f);
    }
}
