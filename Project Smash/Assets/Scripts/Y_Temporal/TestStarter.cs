using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace PSmash.Temporal
{
    public class TestStarter : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Tester());
        }

        IEnumerator Tester()
        {
            yield return new WaitForSeconds(0.5f);
            SceneManager.LoadScene(1);
        }
    }

}
