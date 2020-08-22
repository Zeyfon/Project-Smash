using Cinemachine;
using PSmash.Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCameraEffects : MonoBehaviour
{
    PlayerFighterV2 fighter;
    private void Awake()
    {
        fighter = FindObjectOfType<PlayerFighterV2>();
    }
    private void OnEnable()
    {
        fighter.AirSmashAttackEffect += AirSmashAttack;
    }

    private void OnDisable()
    {
        fighter.AirSmashAttackEffect -= AirSmashAttack;

    }
    public void AirSmashAttack() 
    {
        StartCoroutine(AirSmashAttackCoroutine());
    }

    IEnumerator AirSmashAttackCoroutine()
    {
        Debug.Log("Splash Effects On");
        CinemachineVirtualCamera cam = GetComponent<CinemachineVirtualCamera>();
        CinemachineBasicMultiChannelPerlin noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        noise.m_AmplitudeGain = 1;
        yield return new WaitForSeconds(0.25f);
        float amplitude = noise.m_AmplitudeGain;
        while (noise.m_AmplitudeGain > 0)
        {
            amplitude -= Time.deltaTime * 20;
            if (amplitude <= 0) amplitude = 0;
            noise.m_AmplitudeGain = amplitude;

            yield return new WaitForEndOfFrame();
        }
    }
}
