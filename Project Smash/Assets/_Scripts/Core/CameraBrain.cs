using Cinemachine;
using PSmash.Combat;
using System.Collections;
using UnityEngine;

namespace PSmash.Core
{
    public class CameraBrain : MonoBehaviour
    {
        [SerializeField] float shakeTime = 1;
        //private void OnEnable()
        //{
        //    FindObjectOfType<PlayerFighter>().AirSmashAttackEffect += CameraShake;
        //}

        //private void OnDisable()
        //{
        //    //FindObjectOfType<PlayerFighter>().AirSmashAttackEffect -= CameraShake;
        //}
        public void CameraShake()
        {
            StartCoroutine(CameraShakeCO());
        }

        IEnumerator CameraShakeCO()
        {
            print("Splash Effects On");
            CinemachineVirtualCamera cam = GetComponent<CinemachineVirtualCamera>();
            CinemachineBasicMultiChannelPerlin noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
            noise.m_AmplitudeGain = 2;

            yield return new WaitForSeconds(0.25f);
            float amplitude = noise.m_AmplitudeGain;
            print(amplitude);
            while (noise.m_AmplitudeGain > 0)
            {
                print("Noise");
                amplitude -= Time.deltaTime/shakeTime;
                if (amplitude <= 0) amplitude = 0;
                noise.m_AmplitudeGain = amplitude;
                print(amplitude);
                yield return new WaitForEndOfFrame();
            }
        }
    }

}
