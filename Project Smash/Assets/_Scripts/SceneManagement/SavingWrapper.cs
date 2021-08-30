
using System.Collections;
using UnityEngine;
using GameDevTV.Saving;
using PSmash.Checkpoints;
using PSmash.Movement;
using System;

namespace PSmash.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        //CONFIG
        [SerializeField] float fadeInTime = 0.5f;
        
        //STATE
        const string defaultSaveFile = "PSmashSaveFile";
        _Controller _controller;
        bool cr_Running=false;


        //INITIALIZE
        private void Awake()
        {
            _controller = new _Controller();
        }

        private void Start()
        {
            InitialLoad();
        }

        private void OnEnable()
        {
            _controller.GameManagement.Enable();
            _controller.GameManagement.Save.performed += ctx => Save();
            _controller.GameManagement.Load.performed += ctx => InitialLoad();
            _controller.GameManagement.Delete.performed += ctx => Delete();
        }
        private void OnDisable()
        {
            _controller.GameManagement.Disable();
            _controller.GameManagement.Save.performed -= ctx => Save();
            _controller.GameManagement.Load.performed -= ctx => InitialLoad();
            _controller.GameManagement.Delete.performed -= ctx => Delete();
        }

        void InitialLoad()
        {
            StartCoroutine(LoadLastScene(true));
        }


        //PUBLIC
        public void LoadLastSavedScene()
        {
            StartCoroutine(LoadLastScene(false));
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile, false);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }

        public void QuitGame()
        {
            StartCoroutine(QuitGame_CR());
        }


        //PRIVATE
        IEnumerator LoadLastScene(bool isInitialized)
        {
            if (!cr_Running)
            {
                cr_Running = true;
                if (!isInitialized)
                {
                    GetComponent<SavingSystem>().Save(defaultSaveFile);
                    FindObjectOfType<WorldManager>().IncreaseCheckpointCounter();
                }
                yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile, true, isInitialized);
                GetComponent<SavingSystem>().Save(defaultSaveFile);
                FindObjectOfType<WorldManager>().IncreaseCheckpointCounter();
                UIFader fader = FindObjectOfType<UIFader>();
                fader.FadeOutInmediate();
                yield return fader.FadeIn(fadeInTime);
                cr_Running = false;
            }
            yield return null;
        }

        IEnumerator QuitGame_CR()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
            UIFader fader = FindObjectOfType<UIFader>();
            fader.FadeOutInmediate();
            print("Fading");
            yield return fader.FadeOut(3);
            print("Fade Ended");
            Application.Quit();
        }
    }
}

