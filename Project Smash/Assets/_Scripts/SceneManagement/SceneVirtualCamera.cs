using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace PSmash.SceneManagement
{
    public class SceneVirtualCamera : MonoBehaviour, IChangeVCamera
    {
        [SerializeField]  CinemachineVirtualCamera virtualCamera = null;

        public void EnterCamArea()
        {
            virtualCamera.m_Priority = 20;
        }

        public void ExtiCamArea()
        {
            virtualCamera.m_Priority = 0;
        }
    }
}

