using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
        //CONFIG
        [SerializeField] UnityEvent onSaveStart;
        [SerializeField] UnityEvent OnSaveEnds;

        //STATE
        List<string> restoredObjectsForLoadLastSavedScene = new List<string>();
        int initialSceneBuildIndex = 0;

        /////////////////////////////////////////////////////////////////////PUBLIC//////////////////////////////////////////////////////////////////////
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
        public IEnumerator LoadLastScene(string saveFile, bool isLoadLastScene, bool isInitialized)
        {
            print("Loading Scene");
            //print("Trying to load a scene");
            Dictionary<string, object> state = LoadFile(saveFile);

            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                //print("Last Save Scene Loaded");
                int buildIndex = SceneManager.GetActiveScene().buildIndex;
                buildIndex = (int)state["lastSceneBuildIndex"];

                if (buildIndex > SceneManager.sceneCountInBuildSettings)
                {
                    Debug.LogWarning("The scene you want to load is not set in the Build Settings");
                    //yield return null;
                }
                else 
                {
                    //print("3");
                    print("Loading scene " + buildIndex);
                    yield return SceneManager.LoadSceneAsync(buildIndex);
                    print("Done Loading");
                }
            }
            else
            {
                //print("5");
                if (isInitialized)
                {
                    print("No scene was loaded");
                    initialSceneBuildIndex = SceneManager.GetActiveScene().buildIndex;
                }
                else
                {
                    yield return SceneManager.LoadSceneAsync(initialSceneBuildIndex);
                    print("The same scene was loaded again");
                }
                //Debug.LogWarning("There is no lastSceneBuildIndex");
            }
            RestoreState(state, isLoadLastScene);
            print("LoadLastScene ended");
            OnSaveEnds.Invoke();
            yield return null;
        }

        /// <summary>
        /// Save the current scene to the provided save file.
        /// </summary>
        public void Save(string saveFile)
        {
            print("Saving");
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



        public void Load(string saveFile, bool isLoadLastScene)
        {
            print("Loading");
            RestoreState(LoadFile(saveFile), isLoadLastScene);
        }

        /////////////////////////////////////////////////////////////////////PRIVATE//////////////////////////////////////////////////////////////////////

        Dictionary<string, object> LoadFile(string saveFile)
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

        void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
            print("Saving to " + path);
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        void CaptureState(Dictionary<string, object> state)
        {
            print("Capturing Data");
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                //print("Saving the  " + saveable.gameObject.name);
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }
            Tent checkpoint = FindObjectOfType<Tent>();
            
            if (checkpoint != null && checkpoint.IsPlayerInSavePoint())
            {
                int index = SceneManager.GetActiveScene().buildIndex;
                print("lastSceneBuildIndex  " + index + " was captured");
                state["lastSceneBuildIndex"] = index;
            }
            else
            {
                //Debug.LogWarning("No lastSceneBuildIndex was saved");
            }
        }

        void RestoreState(Dictionary<string, object> state, bool isLoadLastScene)
        {
            print("Restoring Data");
            if (isLoadLastScene)
                restoredObjectsForLoadLastSavedScene.Clear();
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    if (!restoredObjectsForLoadLastSavedScene.Contains(id))
                    {
                        saveable.RestoreState(state[id], true);
                    }
                    else
                    {
                        saveable.RestoreState(state[id], false);
                    }
                    restoredObjectsForLoadLastSavedScene.Add(id);
                }
            }
        }

        string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}