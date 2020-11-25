﻿using UnityEngine;
using System.Collections;

namespace PSmash.Attributes
{
    public class PostureBar : MonoBehaviour
    {
        [SerializeField] Transform guardBar = null;
        [SerializeField] float effectTime = 0.5f;
        [SerializeField] AudioClip guardBarRecoveredSound = null;
        [SerializeField] Transform effectTransform = null;

        EnemyPosture posture;

        private void Awake()
        {
            posture = transform.parent.transform.GetComponentInChildren<EnemyPosture>();
        }

        private void OnEnable()
        {
            posture.OnGuardBarRecoveredAfterStun += GuardBarRecoverEffect;
        }

        private void OnDisable()
        {
            posture.OnGuardBarRecoveredAfterStun -= GuardBarRecoverEffect;
        }

        private void Update()
        {
            guardBar.localScale = new Vector2(posture.GetPosture() / posture.GetInitialPosture(), transform.localScale.y); ;
        }

        void GuardBarRecoverEffect()
        {
            StartCoroutine(GuardBarEffect());
        }

        IEnumerator GuardBarEffect()
        {
            SpriteRenderer renderer = effectTransform.GetComponent<SpriteRenderer>();
            GetComponent<AudioSource>().PlayOneShot(guardBarRecoveredSound);
            float alpha = renderer.color.a;
            while(alpha < 1)
            {
                alpha += Time.deltaTime / effectTime;
                if (alpha > 1) alpha = 1;
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
            }
            yield return new WaitForSeconds(1f);
            alpha = 1;
            while(alpha != 0)
            {
                alpha -= Time.deltaTime / effectTime;
                if (alpha < 0) alpha = 0;
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
            }
        }

    }
}

