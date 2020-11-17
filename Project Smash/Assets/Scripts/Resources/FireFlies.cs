using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Core;

namespace PSmash.Resources
{

    public class FireFlies : MonoBehaviour
    {
        [SerializeField] GameObject carryOnFireFlies = null;

        GameObject carryOnFireFliesClone;
        // Start is called before the first frame update

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                if (carryOnFireFliesClone != null)
                {
                    print("FireFly destroyed");
                    Destroy(carryOnFireFliesClone);
                }

                StartCoroutine(InstantiateFireFlies(collision.transform));
            }
        }

        IEnumerator InstantiateFireFlies(Transform targetTransform)
        {
            carryOnFireFliesClone = Instantiate(carryOnFireFlies,targetTransform.position + new Vector3(0,1,0) ,Quaternion.identity);
            yield return null;
            carryOnFireFliesClone.GetComponent<FollowEntity>().SetData(targetTransform, true);
        }
    }
}

