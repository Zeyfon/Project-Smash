using System.Collections;
using UnityEngine;

namespace PSmash.Movement
{
    public class Platform : MonoBehaviour
    {
        [SerializeField] float speed = 3;
        [SerializeField] Transform initialPosition = null;
        [SerializeField] Transform finalPosition = null;
        //[SerializeField] Transform line = null;

        bool isMovingWall = false;
        bool isMovingOnWall = false;
        PlayerMovement playerMovement;

        Transform initialPlayerParent;

        void Start()
        {
            //if (GetComponent<Wall>() != null) isMovingWall = true;
            //GetLineReady();
            StartCoroutine(StartMovement());
        }

        //void GetLineReady()
        //{
        //    line.position = initialPosition.position;
        //    float distance = Vector3.Distance(finalPosition.position, initialPosition.position);
        //    line.GetComponent<SpriteRenderer>().size = new Vector2(distance, 1);
        //    float x = finalPosition.position.x - initialPosition.position.x;
        //    float y = finalPosition.position.y - initialPosition.position.y;
        //    float angle = Mathf.Atan2(y, x) * 180 / Mathf.PI;
        //    line.eulerAngles = new Vector3(0, 0, angle);
        //}

        IEnumerator StartMovement()
        {
            while (4 > Time.timeSinceLevelLoad)
            {
                yield return new WaitForEndOfFrame();
            }
            Debug.Log("Start Moving");
            StartCoroutine(Movement());
        }

        IEnumerator Movement()
        {
            float step = 0;
            float journeyPercentage = 0;
            float currentJourneyLength = 0;
            float journeyLength;
            while (true)
            {
                journeyLength = Vector3.Distance(finalPosition.position, initialPosition.position);
                step = 0;
                currentJourneyLength = 0;
                while (currentJourneyLength < journeyLength)
                {
                    step += Time.fixedDeltaTime * speed;
                    journeyPercentage = step / journeyLength;
                    transform.position = Vector3.Lerp(initialPosition.position, finalPosition.position, journeyPercentage);
                    currentJourneyLength = Vector3.Distance(initialPosition.position, transform.position);
                    yield return new WaitForFixedUpdate();
                }
                yield return new WaitForSeconds(2);
                journeyLength = Vector3.Distance(finalPosition.position, initialPosition.position);
                step = 0;
                currentJourneyLength = 0;
                while (currentJourneyLength < journeyLength)
                {
                    step += Time.fixedDeltaTime * speed;
                    journeyPercentage = step / journeyLength;
                    transform.position = Vector3.Lerp(finalPosition.position, initialPosition.position, journeyPercentage);
                    currentJourneyLength = Vector3.Distance(finalPosition.position, transform.position);
                    yield return new WaitForFixedUpdate();
                }
                yield return new WaitForSeconds(2);
            }
        }



        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (playerMovement == null) playerMovement = collision.GetComponent<PlayerMovement>();
                if (playerMovement.IsMovingOnWall) ChildPlayerToThisPlatform(collision.transform);
                else
                {
                    ReturnPlayerToBeforeParent(collision.transform);
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                ReturnPlayerToBeforeParent(collision.transform);
            }
        }

        void ChildPlayerToThisPlatform(Transform playerTransform)
        {
            if (initialPlayerParent == null) initialPlayerParent = playerTransform.parent;
            playerTransform.parent = base.transform;
        }

        void ReturnPlayerToBeforeParent(Transform playerTransform)
        {
            if (initialPlayerParent == null)
            {
                Debug.LogWarning("No parent saved for player");
                return;
            }
            playerTransform.parent = initialPlayerParent;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(.8f, .1f, 1, 0.7f);
            Gizmos.DrawWireSphere(finalPosition.position, 0.5f);
            Gizmos.DrawWireSphere(initialPosition.position, 0.5f);
        }
    }

}
