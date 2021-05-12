using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using PSmash.SceneManagement;

namespace PSmash.Temporal
{
    public class InitialScene : MonoBehaviour
    {

        // Start is called before the first frame update
        void Start()
        {
            SceneManager.LoadSceneAsync(1);
            //FindObjectOfType<SavingWrapper>().LoadLastSavedScene();
        }
    }
    // Update is called once per fram
}

