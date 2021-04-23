using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
            SavingWrapper savingWrapper = FindObjectOfType<SavingWrapper>();
            if (savingWrapper)
            {
                savingWrapper.LoadLastScene2();
            }
            else
            {
                SceneManager.LoadScene(1);
            }
        }
    }
    // Update is called once per fram
}

