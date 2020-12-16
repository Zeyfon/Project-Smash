using PSmash.Core;
using PSmash.Attributes;
using PSmash.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSmash.Items.Doors
{
    public class DoorKey : Door
    {
        [Header("Doorkey Items")]
        [SerializeField] GameObject UnlockingDoorMomentKey;
        [SerializeField] int keysRequired = 0;
        [SerializeField] List<Transform> keyPositions = new List<Transform>();

        [Header("Force Option")]
        [SerializeField] bool panBothSides = false;

        public delegate void KeyDoorOpening(InteractionList myValue);
        public static event KeyDoorOpening OnDoorOpening;
        int tracker = 0;
        int currentKeys = 0;

        List<GameObject> unlockingMomentKeys = new List<GameObject>();

        void Awake()
        {
            CircleCollider2D circle = GetComponent<CircleCollider2D>();
            if (circle.enabled) circle.enabled = false;
        }
        void Start()
        {
            // Is in Start to allow the Keys to properly set during the Awake phase
            Key.keys.TryGetValue(doorID, out keysRequired);
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
            followCamera = GameObject.FindGameObjectWithTag("FollowCamera").transform;
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
            yield return new WaitForSeconds(1.75f);
            myVirtualCamera.m_Priority = 0;
            yield return new WaitForSeconds(1f);
            //print("Playercontrol is Enabled");
            EnableAvatarControl();
        }


        //Is the controller of the Opening Door Moment
        //when the player gets near enough for the door to start opening
        //Also here the player control will be removed
        IEnumerator UnlockingDoorMoment(Transform playerTransform)
        {
            DisableAvatarControl();
            //For now disable the UI over the Door Object
            OnDoorOpening(doorID);
            yield return new WaitForSeconds(0.5f);
            yield return KeyCinematics(playerTransform);
            yield return new WaitForSeconds(1);
            yield return CongratulationsMoment();
            EnableAvatarControl();
            yield return new WaitForSeconds(0.5f);
            DisableKeys();
            yield return new WaitForSeconds(1.3f);
            yield return OpenDoor();
        }

        //The controller of the Key Moving to the door Cinematic
        IEnumerator KeyCinematics(Transform player)
        {
            float distance = 2.5f;
            Vector2[] targetPositionsOverPlayer = new Vector2[keysRequired];
            Vector2[] targetPositionsOverDoor = new Vector2[keysRequired];
            float angle = 180 / (targetPositionsOverPlayer.Length + 1);
            float betweenKeysOnDoorDistance = GetComponent<BoxCollider2D>().size.y / (keysRequired + 1);
            for (int i = 0; i < targetPositionsOverPlayer.Length; i++)
            {
                targetPositionsOverPlayer[i] = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (angle * (i + 1))) * distance + player.position.x,
                                                 Mathf.Sin(Mathf.Deg2Rad * (angle * (i + 1))) * distance + player.position.y + 1);
                targetPositionsOverDoor[i] = new Vector2(transform.position.x, transform.position.y + betweenKeysOnDoorDistance * (i + 1));
                // Get the position to where the key will go above the player
                // Then get the position to where it will go over the door
                //Send this info to the SpawnKey Coroutine
                StartCoroutine(SpawnKey(targetPositionsOverPlayer[i], targetPositionsOverDoor[i], player));
            }

            while (tracker != keysRequired)
            {
                yield return new WaitForEndOfFrame();
            }
            audioSource.PlayOneShot(keysLandingSound);
            followCamera.GetComponent<FollowCamera>().CameraShake();
            yield return null;
        }

        //This coroutine is in charge of instantiate each key(There will be a # of coroutines
        //depending on the # of keys for this door
        //Then will send both target position previously gotten to the key and will initiate the movement
        //Each key will perform its own movement in their own script
        //Finally this coroutine will tracking when each key finishes its movement to both targets
        //in order to know when all keys have finished to continue the next step in this moment
        IEnumerator SpawnKey(Vector2 targetPositionOverPlayer, Vector2 targetPositionOverDoor, Transform player)
        {
            GameObject unlockingDoorMomentKeyClone = Instantiate(UnlockingDoorMomentKey, player.position + new Vector3(0, 1, 0), Quaternion.identity, transform.GetChild(0));
            yield return unlockingDoorMomentKeyClone.GetComponent<UnlockingDoorMomentKey>().KeyMoment(targetPositionOverPlayer, targetPositionOverDoor);
            tracker++;
            unlockingMomentKeys.Add(unlockingDoorMomentKeyClone);
            yield return null;
        }

        //Here will be the amusement moment of completing the key quest
        IEnumerator CongratulationsMoment()
        {
            //Congratulations audio + glow on keys
            //Glow banishes
            //Keys disappear
            audioSource.PlayOneShot(congratulationsClip);
            foreach(GameObject currentKey in unlockingMomentKeys)
            {
                currentKey.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                currentKey.transform.GetChild(1).GetComponent<ParticleSystem>().Play();
                //print("Glow Started");
            }
            while (audioSource.isPlaying)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }

        private void DisableKeys()
        {
            foreach (GameObject currentKey in unlockingMomentKeys)
            {
                StartCoroutine(DisableCurrentKey(currentKey));
            }
        }
        IEnumerator DisableCurrentKey(GameObject currentKey)
        {
            print("Disabling Key");
            currentKey.transform.GetChild(1).GetComponent<ParticleSystem>().Stop();
            float alpha = 1;
            SpriteRenderer renderer = currentKey.GetComponent<SpriteRenderer>();
            while(alpha != 0)
            {
                alpha -= Time.deltaTime;
                if (alpha <= 0) alpha = 0;
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, alpha);
                yield return new WaitForEndOfFrame();
            }
            yield return null;
        }
        //The Opening door Mechanic
        //Also all the details added to this mechanic are here (Dust, Audio,etc)
        IEnumerator OpenDoor()
        {
            dustParticles.Play();
            audioSource.clip = doorOpeningSound;
            audioSource.Play();
            float timer = 0;
            //The rate is the colliders height in Unity units
            //and the amount of time it wants to spend opening
            float rate = GetComponent<BoxCollider2D>().size.y / openingTime;
            Transform spriteTransform = transform.GetChild(0);
            //print("Trying to access Disabling Text");
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            float yOffset = collider.offset.y;
            while (timer < openingTime)
            {
                yOffset += Time.deltaTime * rate;
                collider.offset = new Vector2(0, yOffset);
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
                StartCoroutine(UnlockingDoorMoment(collision.transform));
            }
        }
    }
}

