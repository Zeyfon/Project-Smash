﻿using PSmash.Attributes;
using PSmash.Combat.Weapons;
using PSmash.Core;
using System.Collections;
using UnityEngine;

namespace PSmash.Items.Traps
{
    public class Trap : MonoBehaviour, IReturnPosition
    {
        [SerializeField] int damage = 15;
        [SerializeField] float fadeOutTime = 1;
        [SerializeField] float fadeInTime = 1;

        public delegate void PlayerController(bool state);
        public static event PlayerController EnablePlayerController;
        Vector3 lastPosition;
        Weapon weapon;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                IDamagable target = collision.collider.GetComponent<IDamagable>();
                target.TakeDamage(transform, weapon, damage);
                StartCoroutine(ReturnPlayerToLastPosition(collision.transform));
            }
            if (collision.collider.CompareTag("Enemy"))
            {
                IKillable target = collision.collider.GetComponent<IKillable>();
                target.Kill(transform);
            }
        }

        public void SetNewPosition(Vector3 lastPosition)
        {
            this.lastPosition = lastPosition;
            print(this.lastPosition);
        }
        IEnumerator ReturnPlayerToLastPosition(Transform player)
        {

            //Fader fader = FindObjectOfType<Fader>();
            Fader fader = new Fader();
            CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
            print("ReturningPlayer");
            EnablePlayerController(false);
            print("1");

            yield return fader.FadeOut(canvasGroup, fadeOutTime);
            print("1");
            player.position = lastPosition;
            print("1");
            yield return new WaitForSeconds(1);
            print("1");
            yield return fader.FadeIn(canvasGroup, fadeInTime);
            print("1");
            EnablePlayerController(true);
        }
    }
}
