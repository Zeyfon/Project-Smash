﻿using Cinemachine;
using PSmash.Combat;
using System.Collections;
using UnityEngine;

namespace PSmash.Core
{
    public class FollowCamera : MonoBehaviour
    {
        PlayerFighterV2 fighter;
        private void Awake()
        {
            fighter = FindObjectOfType<PlayerFighterV2>();
            CinemachineVirtualCamera camera = GetComponent<CinemachineVirtualCamera>();
            camera.m_Follow = fighter.transform.GetComponentInChildren<FollowCameraTarget>().transform;
        }

        private void OnEnable()
        {
            fighter.AirSmashAttackEffect += CameraShake;
        }

        private void OnDisable()
        {
            fighter.AirSmashAttackEffect -= CameraShake;

        }
        public void CameraShake()
        {
            StartCoroutine(CameraShakeCO());
        }

        IEnumerator CameraShakeCO()
        {
            Debug.Log("Splash Effects On");
            CinemachineVirtualCamera cam = GetComponent<CinemachineVirtualCamera>();
            CinemachineBasicMultiChannelPerlin noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            noise.m_AmplitudeGain = 1;
            yield return new WaitForSeconds(0.25f);
            float amplitude = noise.m_AmplitudeGain;
            while (noise.m_AmplitudeGain > 0)
            {
                amplitude -= Time.deltaTime * 500;
                if (amplitude <= 0) amplitude = 0;
                noise.m_AmplitudeGain = amplitude;

                yield return new WaitForEndOfFrame();
            }
        }
    }

}