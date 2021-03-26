using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.SceneManagement;

namespace PSmash.Temporal
{
    public class DeadScene : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(TestStarter());
        }

        IEnumerator TestStarter()
        {
            yield return new WaitForSeconds(1);
            FindObjectOfType<SavingWrapper>().LoadLastScene2();
        }
        // Update is called once per fram
    }
}

