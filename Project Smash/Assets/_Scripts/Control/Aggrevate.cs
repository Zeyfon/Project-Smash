using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Control
{
    public class Aggrevate : MonoBehaviour
    {

        PlayMakerFSM pm;
        Coroutine coroutine;

        private void Awake()
        {
            string aiControllerName = "AIController";
            bool isFound = false;
            foreach(PlayMakerFSM pm in GetComponents<PlayMakerFSM>())
            {
                if (pm.FsmName == aiControllerName)
                {
                    isFound = true;
                    this.pm = pm;
                    break;
                }
            }
            if (!isFound)
                Debug.LogWarning("No " + aiControllerName + "  found in  " + gameObject.name + " .Please verify the fsm is and the name is as it must be");
        }


        /// <summary>
        /// This method is used by the AIController in all enemies
        /// This action will only be triggered in the AIController in the corresponding states
        /// The aggrevatingTime will be the time since the state starts that the enemy will be able to call to combat its surrounding companions
        /// The time must be less than the state action for it to stop aggrevating others and start an endless loop of aggrevating others and them aggrevate this and this turns into a infinite cycle
        /// </summary>
        public void AggrevateNearbyEnemies()
        {
            float aggrevatingTime = 1.5f;
            //print("Looking to aggrevate enemies");
            if (coroutine != null)
                StopCoroutine(coroutine);
            coroutine = StartCoroutine(AggrevateForALimitedTime(aggrevatingTime));
        }

        IEnumerator AggrevateForALimitedTime(float aggrevatingTime)
        {
            float time = 0;
            while (time < aggrevatingTime)
            {
                time += Time.deltaTime;
                AggrevateEnemies();
                yield return null;
            }
        }

        void AggrevateEnemies()
        {
            //print("Aggrevating enemies");
            Collider2D[] colls = Physics2D.OverlapCircleAll(transform.position + new Vector3(0, 1, 0), 4);

            if (colls.Length == 0)
            {
                //print("No Enemies in front to aggrevate");
            }
            foreach (Collider2D coll in colls)
            {
                Aggrevate target = coll.GetComponent<Aggrevate>();
                if (target == null || target == this)
                    continue;
                //print("Aggrevating  this one  " + coll.gameObject.name);
                target.pm.SendEvent("AGGREVATED");
            }
        }
    }

}
