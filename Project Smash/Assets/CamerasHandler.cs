using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using PSmash.Combat;
using System;

namespace PSmash.Core
{
    public class CamerasHandler : MonoBehaviour
    {
        [SerializeField] CinemachineVirtualCamera followCamera = null;
        [SerializeField] CinemachineVirtualCamera finisherCamera = null;
        // Start is called before the first frame update
        void Start()
        {
            EnableFollowCamera();
        }

        private void OnEnable()
        {
            PlayerFighter.onFinisherCamera += CameraSet;
            PlayerFighter.onCameraShake += CameraShake;
        }


        private void OnDisable()
        {
            PlayerFighter.onFinisherCamera -= CameraSet;
            PlayerFighter.onCameraShake -= CameraShake;

        }

        void CameraShake()
        {
            finisherCamera.GetComponent<CameraBrain>().CameraShake();
        }


        void CameraSet(bool enableFinisherCamera)
        {
            if (enableFinisherCamera)
                EnableFinisherCamera();
            else
                EnableFollowCamera();
        }

        void EnableFinisherCamera()
        {
            finisherCamera.m_Priority = 10;
            followCamera.m_Priority = 0;
        }
        void EnableFollowCamera()
        {
            finisherCamera.m_Priority = 0;
            followCamera.m_Priority = 10;
        }
    }
}

