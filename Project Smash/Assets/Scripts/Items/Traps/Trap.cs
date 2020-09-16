using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Combat;
using PSmash.Resources;
using PSmash.SceneManagement;

namespace PSmash.Items.Traps
{
    public class Trap : MonoBehaviour, IReturnPosition
    {
        [SerializeField] float fadeOutTime = 1;
        [SerializeField] float fadeInTime = 1;

        public delegate void PlayerController(bool state);
        public static event PlayerController EnablePlayerController;
        Vector3 lastPosition;
        int damage = 10;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                IDamagable target = collision.collider.GetComponent<IDamagable>();
                target.TakeDamage(transform, damage);
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
            print("ReturningPlayer");
            EnablePlayerController(false);
            print("1");
            Fader fader = FindObjectOfType<Fader>();
            yield return fader.FadeOut(fadeOutTime);
            print("1");
            player.position = lastPosition;
            print("1");
            yield return new WaitForSeconds(1);
            print("1");
            yield return fader.FadeIn(fadeInTime);
            print("1");
            EnablePlayerController(true);
        }
    }
}

