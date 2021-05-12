
using System.Collections;
using UnityEngine;
using GameDevTV.Saving;
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


        //PRIVATE
        IEnumerator LoadLastScene(bool isInitialized)
        {
            if (!cr_Running)
            {
                cr_Running = true;
                if (!isInitialized)
                {
                    GetComponent<SavingSystem>().Save(defaultSaveFile);
                }
                yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile, true, isInitialized);
                GetComponent<SavingSystem>().Save(defaultSaveFile);
                UIFader fader = FindObjectOfType<UIFader>();
                fader.FadeOutInmediate();
                yield return fader.FadeIn(fadeInTime);
                cr_Running = false;
            }
            yield return null;
        }
    }
}

