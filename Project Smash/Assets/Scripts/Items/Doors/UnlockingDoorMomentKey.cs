using System.Collections;
using UnityEngine;

namespace PSmash.Items.Doors
{
    public class UnlockingDoorMomentKey : MonoBehaviour
    {
        [SerializeField] AudioClip appearingClip = null;
        [SerializeField] AudioClip movingClip = null;
        [SerializeField] AudioClip landingClip = null;
        [SerializeField] float movingTime = 2f;

        //This is the key controller moment 
        //Controls the movements for their respective targets
        //Have an amusement moment right after 
        //getting to the target position above the player
        public IEnumerator KeyMoment(Vector2 targetPositionOverPlayer, Vector2 targetPositionOverDoor)
        {
            GetComponent<AudioSource>().PlayOneShot(appearingClip);
            yield return MoveTowardsNextTarget(targetPositionOverPlayer, 0.045f, true);
            KeyShowAmusementMoment();
            yield return new WaitForSeconds(2);
            GetComponent<AudioSource>().Play();
            yield return MoveTowardsNextTarget(targetPositionOverDoor, 0.07f,false);
            yield return null;
        }

        //This is where the the amusement moment for each key will be
        //For now it only shows a Particle System
        private void KeyShowAmusementMoment()
        {
            transform.GetChild(0).GetComponent<ParticleSystem>().Play();
        }

        //The Coroutine in charge of the movement for both targets 
        IEnumerator MoveTowardsNextTarget(Vector3 targetPosition, float lerpValue, bool useLerp)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            float speed = (Vector3.Distance(targetPosition, transform.position) / movingTime);
            print(speed);
            while (Vector3.Distance(transform.position, targetPosition) > 0.15f)
            {
                if (useLerp) transform.position = Vector2.Lerp(transform.position, targetPosition, lerpValue * Time.deltaTime*100);
                else transform.position += direction * speed * Time.deltaTime; 
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }
    }
}

