using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using PSmash.Checkpoints;

namespace PSmash.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "PSmashSaveFile" ;
        [SerializeField] float fadeInTime = 0.2f;

        _Controller _controller;
        bool cr_Running=false;

        private void Awake()
        {
            _controller = new _Controller();
        }

        private void Start()
        {
            LoadLastSavedScene();
        }

        private void OnEnable()
        {
            _controller.GameManagement.Enable();
            _controller.GameManagement.Save.performed += ctx => Save();
            _controller.GameManagement.Load.performed += ctx => Load();
            _controller.GameManagement.Delete.performed += ctx => Delete();
        }

        public void LoadLastSavedScene()
        {
            StartCoroutine(LoadLastScene());
        }

        IEnumerator LoadLastScene()
        {
            if (!cr_Running)
            {
                cr_Running = true;
                yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
                UIFader fader = FindObjectOfType<UIFader>();
                fader.FadeOutInmediate();
                yield return fader.FadeIn(fadeInTime);
                cr_Running = false;
            }
            yield return null;
        }

        public void LoadDeadScene(Tent tent)
        {
            StartCoroutine(LoadDeadSceneCR(tent));
        }

        IEnumerator LoadDeadSceneCR(Tent tent)
        {
            UIFader fader = FindObjectOfType<UIFader>();
            yield return fader.FadeOut(2);
            tent.ResetPlayer();
            GetComponent<SavingSystem>().Save(defaultSaveFile);
            yield return GetComponent<SavingSystem>().LoadDeadScene(0,defaultSaveFile);
            fader.FadeOutInmediate();
            yield return fader.FadeIn(2);
        }

        public void Save()
        {
            GetComponent<SavingSystem>().Save(defaultSaveFile);
        }

        public void Load()
        {
            GetComponent<SavingSystem>().Load(defaultSaveFile);
        }

        public void Delete()
        {
            GetComponent<SavingSystem>().Delete(defaultSaveFile);
        }
    }
}

