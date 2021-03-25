using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PSmash.Saving
{
    public class SavingSystem : MonoBehaviour
    {
 
        public IEnumerator LoadLastScene(string defaultSaveFile)
        {
            //int buildIndex = 0;

            //if (ES3.KeyExists("lastSceneBuildIndex"))
            //{
            //    buildIndex = (int)ES3.Load("lastSceneBuildIndex");
            //    //print(buildIndex);
            //}
            //if(buildIndex != SceneManager.GetActiveScene().buildIndex && buildIndex != 0)
            //yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreState();
            yield return null;
        }
        public void Save(string defaultSaveFile)
        {
            CaptureState();
        }

        void CaptureState()
        {
            foreach(SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                print("Capturing  " + saveable.gameObject.name);
                saveable.CaptureState();
            }
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            ES3.Save("lastSceneBuildIndex", buildIndex);
        }

        public void Load(string defaultSaveFile)
        {
            RestoreState();
        }

        private void RestoreState()
        {
            if (ES3.FileExists())
            {
                foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
                {
                    print("Restoring " + saveable.gameObject.name);
                    saveable.RestoreState();
                }
            }
        }

        public void Delete(string defaultSaveFile)
        {
            ES3.DeleteFile(defaultSaveFile);

        }
    }

}
