using UnityEngine;
using GameDevTV.Saving;
using System.Collections;
using PSmash.SceneManagement;
using PSmash.Core;

namespace PSmash.Inventories
{
    public class Mace : MonoBehaviour,ISaveable
    {
        [SerializeField] SubWeaponItem subWeapon = null;
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] CanvasGroup canvasGroup;
        bool hasBeenTaken = false;

        public delegate void ObjectTaken(bool isEnabled);
        public static event ObjectTaken onObjectTaken;

        void MaceTaken()
        {
            hasBeenTaken = true;
            GetComponentInChildren<SpriteRenderer>().enabled = false;
            GetComponent<Collider2D>().enabled = false;
            StartCoroutine(MaceTakenCoroutine());
        }

        IEnumerator MaceTakenCoroutine()
        {
            print("Mace Starting");
            onObjectTaken(false);
            audioSource.Play();
            Fader fader = new Fader();
            yield return fader.FadeIn(canvasGroup, 0.5f);
            yield return new WaitForSeconds(4);
            yield return fader.FadeOut(canvasGroup, 1);
            FindObjectOfType<SavingWrapper>().Save();
            onObjectTaken(true);
            gameObject.SetActive(false);
            print("Mace Ending");
        }
        public object CaptureState()
        {
            return hasBeenTaken;
        }

        public void RestoreState(object state)
        {
            hasBeenTaken = (bool)state;
            if(hasBeenTaken)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponent<Equipment>().SetSubWeapon(subWeapon);
                MaceTaken();
            }
        }
    }

}
