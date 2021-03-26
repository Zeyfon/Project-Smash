using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace PSmash.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        [SerializeField] UnityEvent onStartSaving;
        [SerializeField] UnityEvent onEndSaving;
        public IEnumerator LoadLastScene(string defaultSaveFile)
        {
            if (ES3.FileExists()) 
            {
                //int buildIndex = SceneManager.GetActiveScene().buildIndex;
                //if (state.ContainsKey("lastSceneBuildIndex"))
                //{
                //    buildIndex = (int)state["lastSceneBuildIndex"];
                //}
                //yield return SceneManager.LoadSceneAsync(buildIndex);
                RestoreState();
            }
            yield return null;
        }
        public void Save(string defaultSaveFile)
        {
            print("Saving");
            onStartSaving.Invoke();
            CaptureState();
            onEndSaving.Invoke();
        }

        public void Load(string defaultSaveFile)
        {
            if (ES3.FileExists())
            {
                print("Loading");
                Dictionary<string, object> state = (Dictionary<string, object>)ES3.Load(defaultSaveFile);
                RestoreState();
            }
        }

        public void Delete(string defaultSaveFile)
        {
            print("Deleting file");
            ES3.DeleteFile(defaultSaveFile);
        }

        private void CaptureState()
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                saveable.CaptureState();
            }
            ES3.Save("lastSceneIndex", SceneManager.GetActiveScene().buildIndex);
        }


        private void RestoreState()
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                saveable.RestoreState();
            }
        }

    }
}
