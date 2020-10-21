using UnityEngine;
using Cinemachine;
using PSmash.Combat;
using PSmash.Resources;

namespace PSmash.Items.Doors
{
    public class Door : MonoBehaviour, IDamagable
    {
        [Header("Force Open Door")]
        [Tooltip("Press to Force door to open. DEBUGGING USE")]
        [SerializeField] protected bool opendoor = false;

        [Header("General Items")]
        [Tooltip("ID used to match with the keys to know how many keys are required for this door")]
        [SerializeField] public InteractionList doorID;
        [Tooltip("Time the door will take to fully open")]
        [SerializeField] protected float openingTime = 5;
        [SerializeField] protected AudioSource audioSource;
        [SerializeField] protected AudioClip congratulationsClip = null;
        [SerializeField] protected AudioClip doorFullyOpenedSound = null;
        [SerializeField] protected AudioClip doorOpeningSound = null;
        [SerializeField] protected AudioClip hitOnDoorSound = null;
        [SerializeField] protected AudioClip keysLandingSound = null;
        [SerializeField] protected ParticleSystem dustParticles = null;


        [Header("Camera Items")]
        [SerializeField] protected Transform followCamera = null;
        [SerializeField] protected CinemachineVirtualCamera myVirtualCamera;
        [SerializeField] protected float fadeInTime = 2;
        [SerializeField] protected float fadeOutTime = 2;

        public delegate void DoorCinematic(bool state);
        public static event DoorCinematic EnablePlayerController;

        // Used for the child Classes to force the door to open
        protected virtual void ForceOpenDoor()
        {
            //Filled by children classes
        }

        //Proxy used by child class to enable playercontroller in InputHandler script
        protected void EnableAvatarControl()
        {
            if (EnablePlayerController == null) return;
            EnablePlayerController(true);
        }

        //Proxy used by child class to disable playercontroller in InputHandler script
        protected void DisableAvatarControl()
        {
            if (EnablePlayerController == null) return;
            EnablePlayerController(false);
        }

        public void TakeDamage(Transform attacker, int damage)
        {
            audioSource.PlayOneShot(hitOnDoorSound);
        }
    }

}
