using UnityEngine;
using GameDevTV.Saving;
using System.Collections;
using PSmash.SceneManagement;
using PSmash.Core;

namespace PSmash.Inventories
{
    public class Mace : MonoBehaviour,ISaveable
    {
        [SerializeField] Subweapon subWeapon = null;
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
            yield return fader.FadeOut(canvasGroup, 0.5f);
            yield return new WaitForSeconds(4);
            yield return fader.FadeIn(canvasGroup, 1);
            FindObjectOfType<SavingWrapper>().Save();
            onObjectTaken(true);
            gameObject.SetActive(false);
            print("Mace Ending");
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                collision.GetComponent<Equipment>().SetSubWeapon(subWeapon);
                MaceTaken();
            }
        }

        public object CaptureState()
        {
            print("Capture Mace Object " + hasBeenTaken);
            return hasBeenTaken;
        }

        public void RestoreState(object state, bool isLoadLastSavedScene)
        {
            hasBeenTaken = (bool)state;
            print("Restore Mace Object " + hasBeenTaken);

            if (hasBeenTaken)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
    }

}
