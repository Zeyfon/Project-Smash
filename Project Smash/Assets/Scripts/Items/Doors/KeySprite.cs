using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PSmash.Items.Doors
{
    public class KeySprite : MonoBehaviour
    {
        [SerializeField] AudioClip appearingClip = null;
        [SerializeField] AudioClip movingClip = null;
        [SerializeField] AudioClip landingClip = null;
        [SerializeField] float movingTime = 2;
        AudioSource audioSource;
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public IEnumerator KeyMovement(Vector3 targetPosition)
        {
            Vector3 tempTarget = transform.position + new Vector3(0, 1.5f, 0);
            audioSource.PlayOneShot(appearingClip);

            yield return MoveTowardsNextTarget(tempTarget);
            //print("Moved Upwards");
            yield return new WaitForSeconds(1);
            audioSource.Play();
            yield return MoveTowardsNextTarget(targetPosition);
            //print("Key Moved Towards Target");
            //audioSource.Stop();
            audioSource.PlayOneShot(landingClip);
            yield return null;
        }

        IEnumerator MoveTowardsNextTarget(Vector3 targetPosition)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            float speed = Vector3.Distance(targetPosition, transform.position) / movingTime;
            while (Vector3.Distance(transform.position, targetPosition) > 0.05f)
            {
                transform.position = Vector2.Lerp(transform.position, targetPosition, 0.05f);
                yield return new WaitForEndOfFrame();
            }
            //print("KeyPositioned");
            yield return null;
        }
    }
}

