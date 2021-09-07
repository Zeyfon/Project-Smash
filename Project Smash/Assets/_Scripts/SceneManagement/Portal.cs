using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;

namespace PSmash.SceneManagement
{
    public class Portal : MonoBehaviour
    {

        enum DestinationIdentifier
        {
            A,B,C,D,E
        }

        [SerializeField] int sceneToLoad=1;
        [SerializeField] Transform spawnPoint;
        [SerializeField] DestinationIdentifier destination;

        [SerializeField] float fadeInTime = 2f;
        [SerializeField] float fadeOutTime = 1f;
        [SerializeField] float fadeWaitTime = 1f;


        public delegate void SceneTransition(bool isEnabled);
        public static event SceneTransition OnPortalTriggered;

        private void OnTriggerEnter2D(Collider2D other)
        {
            print("Portal Triggered");

            if (other.CompareTag("Player"))
            {
                print("Player detected in this portal "+ gameObject.name);
                StartCoroutine(Transition());
            }
        }

        IEnumerator Transition()
        {
            if(sceneToLoad < 0)
            {
                Debug.LogError(" Scene to Load not set");
                yield break;
            }
            DontDestroyOnLoad(gameObject);

            UIFader fader = FindObjectOfType<UIFader>();
            print(fader.gameObject.name);
            if(OnPortalTriggered != null)
            {
                OnPortalTriggered(false);
            }
            else
            {
                Debug.LogWarning("Portal cannot disable current Player controller");
            }
            yield return fader.FadeOut(fadeOutTime);

            
            SavingWrapper savingWrapper = GameObject.FindObjectOfType<SavingWrapper>();
            savingWrapper.Save();

            ////////////////////////////////////////////////////// NEXT SCENE LOADING /////////////////////////////////////////

            yield return SceneManager.LoadSceneAsync(sceneToLoad);

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            if (OnPortalTriggered != null)
                OnPortalTriggered(false);
            else
                Debug.LogWarning("Portal cannot disable new Player controller");

            print("Other Scene Loaded");
            savingWrapper.Load();
            Portal otherPortal = GetOtherPortal();
            if(otherPortal != null)
            {
                UpdatePlayerPosition(otherPortal);

                savingWrapper.Save();

                yield return new WaitForSeconds(fadeWaitTime);

                yield return fader.FadeIn(fadeInTime);

                if (OnPortalTriggered != null)
                    OnPortalTriggered(true);
                else
                    Debug.LogWarning("Portal cannot enable new Player controller");
            }
            //else
            //{
            //    Destroy(FindObjectOfType<SavingWrapper>().transform.parent.gameObject, 0.1f);
            //}
            Destroy(gameObject);
        }

        Portal GetOtherPortal()
        {
            foreach(Portal portal in FindObjectsOfType<Portal>())
            {
                if (portal == this) continue;
                if(portal.destination != this.destination) continue;
                return portal;
            }
            return null;
        }

        void UpdatePlayerPosition(Portal otherPortal)
        {
            GameObject player = GameObject.FindWithTag("Player");
            //print(otherPortal.gameObject.name + "  " + otherPortal.spawnPoint.position);
            player.transform.position = otherPortal.spawnPoint.position;
            //TODO 
            //We need to properly set the orientation of the player accordingly to the spawn orientation
            //This must be done within the Player Movemenet Script using the Flip Method
        }
    }
}

