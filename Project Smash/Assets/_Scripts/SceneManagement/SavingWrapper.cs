using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;


namespace PSmash.SceneManagement
{
    public class SavingWrapper : MonoBehaviour
    {
        const string defaultSaveFile = "PSmashSaveFile" ;
        [SerializeField] float fadeInTime = 0.2f;

        _Controller _controller;
        private void Awake()
        {
            _controller = new _Controller();
        }

        private void Start()
        {
            StartCoroutine(LoadLastScene());
        }

        private void OnEnable()
        {
            _controller.GameManagement.Enable();
            _controller.GameManagement.Save.performed += ctx => Save();
            _controller.GameManagement.Load.performed += ctx => Load();
            _controller.GameManagement.Delete.performed += ctx => Delete();
        }

        IEnumerator LoadLastScene()
        {
            yield return GetComponent<SavingSystem>().LoadLastScene(defaultSaveFile);
            //UIFader fader = FindObjectOfType<UIFader>();
            //fader.FadeOutInmediate();
            //yield return fader.FadeIn(fadeInTime);
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

