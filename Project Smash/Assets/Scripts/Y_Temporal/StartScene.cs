using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PSmash.Temporal
{
    public class StartScene : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(TestStarter());
        }

        IEnumerator TestStarter()
        {
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(1);
        }
        // Update is called once per fram
    }
}

