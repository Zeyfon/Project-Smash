using PSmash.Attributes;
using PSmash.Combat.Weapons;
using UnityEngine;

namespace PSmash.Combat
{
    public class PlayerGuard : MonoBehaviour
    {
        //CONFIG
        [SerializeField] int damage = 30;
        [SerializeField] AudioSource audioSource = null;
        [SerializeField] AudioClip parrySound = null;
        [SerializeField] AudioClip guardSound = null;
        [Range(0,2)]
        [SerializeField] float parryTime = 1;
      
        //STATE
        bool canParry=false;
        float parryTimer = 0;
        bool isGuarding = false;



        //////////////////////////////////////////////////////////////////////////////////////////    PUBLIC /////////////////////////////////////////////////////////////////////////////////////
        public void EnableGuard()
        {
            parryTimer = 0;
            isGuarding = true;
            print("Guard Enabled  " + isGuarding);
        }

        public void DisableGuard()
        {
            parryTimer = Mathf.Infinity;
            isGuarding = false;
            print("Guard Disabled  " + isGuarding);
        }

        public bool IsGuarding(Transform attacker, Weapon weapon)
        {
            print("Checking guard" + isGuarding);
            if (isGuarding)
            {
                if (canParry)
                {
                    foreach (PlayMakerFSM pm in GetComponentsInParent<PlayMakerFSM>())
                    {
                        if (pm.FsmName == "GuardParryState")
                        {
                            print("Found guard fsm");
                            attacker.GetComponent<IDamagable>().TakeDamage(transform, weapon, AttackType.NotUnblockable, damage);
                            pm.SendEvent("PARRY");
                            PlaySound(parrySound);
                            break;
                        }
                    }
                }
                else
                {
                    PlaySound(guardSound);
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////PRIVATE////////////////////////////////////////////////////////////////////////////////////////////

        void Update()
        {
            if (parryTimer < parryTime)
                canParry = true;
            else
                canParry = false;
            parryTimer += Time.deltaTime;
        }

        void PlaySound(AudioClip sound)
        {
            audioSource.clip = sound;
            audioSource.pitch = Random.Range(0.7f, 1);
            audioSource.Play();
        }
    }
}

