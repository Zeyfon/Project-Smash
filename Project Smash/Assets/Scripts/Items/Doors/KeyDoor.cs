using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PSmash.Resources;
using PSmash.SceneManagement;
using System;

namespace PSmash.Items.Doors
{
    public class KeyDoor : Door
    {
        [Header("Doorkey Items")]
        [SerializeField] GameObject keySprite;
        [SerializeField] int keysRequired = 0;
        [SerializeField] List<Transform> keyPositions = new List<Transform>();

        [Header("Force Option")]
        [SerializeField] bool panBothSides = false;

        public delegate void KeyDoorOpening(InteractionList myValue);
        public static event KeyDoorOpening OnDoorOpening;

        int currentKeys = 0;

        void Awake()
        {
            CircleCollider2D circle = GetComponent<CircleCollider2D>();
            if (circle.enabled) circle.enabled = false;
        }
        void Start()
        {
            // Is in Start to allow the Keys to properly set during the Awake phase
            Key.keys.TryGetValue(doorID, out keysRequired);
            //print(keysRequired + "  " + gameObject.name);
            GetComponentInChildren<PSmash.UI.InGameKeyDoorUI>().InitializeUI(currentKeys, keysRequired);
        }

        //The keys aquired check up is through an event from Key Script
        private void OnEnable()
        {
            Key.OnKeyTaken += KeyTaken;
        }

        private void OnDisable()
        {
            Key.OnKeyTaken -= KeyTaken;
        }

        void Update()
        {
            //Force the door to open
            if (opendoor)
            {
                StartCoroutine(OpenDoor());
                opendoor = false;
            }
        }

        //Force OpenDoor Method
        protected override void ForceOpenDoor()
        {
            StartCoroutine(OpenDoor());
        }

        //Check how many keys have been aquired for this door
        //and check if the required quantity have been reached.
        void KeyTaken(InteractionList keyValue)
        {
            if (keyValue != doorID) return;
            AddKey();
            if (currentKeys >= keysRequired)
            {
                AllKeysAquiredMoment();
            }
        }

        //Add the key recently aquired to the counter
        private void AddKey()
        {
            currentKeys++;
            GetComponentInChildren<PSmash.UI.InGameKeyDoorUI>().UpdateUI(currentKeys);
        }

        //This will enable the door player interactin when the player gets nearby
        //Also will show to the playe with a panning where the door is
        private void AllKeysAquiredMoment()
        {
            if (panBothSides)
            {
                StartCoroutine(ShowDoorMomentPanningBothSides());
            }
            else
            {
                StartCoroutine(ShowDoorMomentInmediateReturn());
            }        
            GetComponent<CircleCollider2D>().enabled = true;
        }

        //Control all the camera movement and
        //Enable-disable player controller
        //during the moment of showing the door to the player
        IEnumerator ShowDoorMomentInmediateReturn()
        {
            Fader fader = FindObjectOfType<Fader>();
            DisableAvatarControl();
            yield return new WaitForSeconds(1);
            myVirtualCamera.m_Priority = 100;
            yield return new WaitForSeconds(4);
            yield return fader.FadeOut(fadeOutTime);
            myVirtualCamera.m_Priority = 0;
            yield return new WaitForSeconds(0.5f);
            yield return fader.FadeIn(fadeInTime);
            EnableAvatarControl();
        }

        IEnumerator ShowDoorMomentPanningBothSides()
        {
            Fader fader = FindObjectOfType<Fader>();
            DisableAvatarControl();
            yield return new WaitForSeconds(1);
            myVirtualCamera.m_Priority = 100;
            yield return new WaitForSeconds(4);
            //yield return fader.FadeOut(fadeOutTime);
            myVirtualCamera.m_Priority = 0;
            yield return new WaitForSeconds(3f);
            //yield return fader.FadeIn(fadeInTime);
            print("Playercontrol is Enabled");
            EnableAvatarControl();
        }


        //Is the controller of the Opening Door Moment
        //when the player gets near enough for the door to start opening
        //Also here the player control will be removed
        IEnumerator OpenDoorMoment(Transform playerTransform)
        {
            DisableAvatarControl();
            OnDoorOpening(doorID);
            yield return new WaitForSeconds(0.5f);
            yield return KeyCinematics(playerTransform);
            yield return CongratulationsMoment();
            EnableAvatarControl();
            yield return OpenDoor();
        }
        //Here will be the amusement moment of completing the key quest
        IEnumerator CongratulationsMoment()
        {
            audioSource.PlayOneShot(congratulationsClip);
            while (audioSource.isPlaying)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }

        //The controller of the Key Moving to the door Cinematic
        IEnumerator KeyCinematics(Transform player)
        {
            Vector3 keyPositionOverdoor = transform.position + new Vector3(0,GetComponent<BoxCollider2D>().size.y/2);
            print("Key Cinematics Started");
            //print(keyPositions.Count);
            for (int i = 0; i < keysRequired; i++)
            {
                yield return PutThisKeyOverTheDoor(keyPositionOverdoor, player);
                yield return new WaitForSeconds(1);
            }
            print("Key List Finished");
            yield return null;
        }

        //The actual Movement by each key in the KeyCinematic Moment
        IEnumerator PutThisKeyOverTheDoor(Vector3 targetPosition, Transform player)
        {
            GameObject keySpriteClone = Instantiate(keySprite, player.position + new Vector3(0, 1, 0), Quaternion.identity, transform.GetChild(0));
            yield return keySpriteClone.GetComponent<KeySprite>().KeyMovement(targetPosition);
            yield return null;
        }

        //The Opening door Mechanic
        //Also all the details added to this mechanic are here (Dust, Audio,etc)
        IEnumerator OpenDoor()
        {
            yield return new WaitForSeconds(1);
            dustParticles.Play();
            audioSource.clip = doorOpeningSound;
            audioSource.Play();
            float timer = 0;
            //The rate is the colliders height in Unity units
            //and the amount of time it wants to spend opening
            float rate = GetComponent<BoxCollider2D>().size.y / openingTime;
            Transform spriteTransform = transform.GetChild(0);
            print("Trying to access Disabling Text");
            while (timer < openingTime)
            {
                spriteTransform.position = new Vector3(spriteTransform.position.x, spriteTransform.position.y + (Time.deltaTime * rate));
                timer += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            audioSource.Stop();
            audioSource.PlayOneShot(doorFullyOpenedSound, 1);
            foreach (ParticleSystem particles in GetComponentsInChildren<ParticleSystem>())
            {
                particles.Stop();
            }
            GetComponent<BoxCollider2D>().enabled = false;
            yield return null;
        }

        // Trigger to detect the player once all keys have been obtained
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                GetComponent<CircleCollider2D>().enabled = false;
                StartCoroutine(OpenDoorMoment(collision.transform));
            }
        }
    }
}

