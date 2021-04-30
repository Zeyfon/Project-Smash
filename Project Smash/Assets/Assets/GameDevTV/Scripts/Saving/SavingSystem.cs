using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using PSmash.Checkpoints;

namespace GameDevTV.Saving
{

    /// <summary>
    /// This component provides the interface to the saving system. It provides
    /// methods to save and restore a scene.
    ///
    /// This component should be created once and shared between all subsequent scenes.
    /// </summary>
    public class SavingSystem : MonoBehaviour
    {
        [SerializeField] UnityEvent onSaveStart;
        [SerializeField] UnityEvent OnSaveEnds;


        public IEnumerator LoadDeadScene(int buildIndex, string saveFile)
        {
            Save(saveFile);
            yield return SceneManager.LoadSceneAsync(buildIndex);
        }
        /// <summary>
        /// Will load the last scene that was saved and restore the state. This
        /// must be run as a coroutine.
        /// </summary>
        /// <param name="saveFile">The save file to consult for loading.</param>
        public IEnumerator LoadLastScene(string saveFile)
        {
            print("Trying to load a scene");
            Dictionary<string, object> state = LoadFile(saveFile);

            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                print("Last Save Scene Loaded");
                int buildIndex = SceneManager.GetActiveScene().buildIndex;
                buildIndex = (int)state["lastSceneBuildIndex"];

                if (buildIndex > SceneManager.sceneCountInBuildSettings)
                {
                    print("2");
                    yield return null;
                }
                else if (buildIndex != SceneManager.GetActiveScene().buildIndex)
                {
                    print("3");
                    print("Loading scene " + buildIndex);
                    yield return SceneManager.LoadSceneAsync(buildIndex);
                    //AsyncOperation asyncOperation =  SceneManager.LoadSceneAsync(buildIndex);
                    //while (!asyncOperation.isDone)
                    //{
                    //    print("Not Done Loading");
                    //    yield return null;
                    //}
                    print("Done Loading");
                    //yield return new WaitForEndOfFrame();
                }
                RestoreState(state);
            }
            else
            {
                print("5");
                //yield return SceneManager.LoadSceneAsync(1);
            }
            print("LoadLastScene ended");
            OnSaveEnds.Invoke();
        }

        /// <summary>
        /// Save the current scene to the provided save file.
        /// </summary>
        public void Save(string saveFile)
        {
            onSaveStart.Invoke();
            Dictionary<string, object> state = LoadFile(saveFile);
            CaptureState(state);
            SaveFile(saveFile, state);
            OnSaveEnds.Invoke();
        }

        /// <summary>
        /// Delete the state in the given save file.
        /// </summary>
        public void Delete(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Deleting from  " + path);
            File.Delete(path);
        }



        public void Load(string saveFile)
        {
            RestoreState(LoadFile(saveFile));
        }

        // PRIVATE

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                //print("Saving the  " + saveable.gameObject.name);
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }
            PlayerPositionCheckpoint checkpoint = FindObjectOfType<PlayerPositionCheckpoint>();
            if (checkpoint != null && checkpoint.IsPlayerInSavePoint())
            {
                state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
            }
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    saveable.RestoreState(state[id]);
                }
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}