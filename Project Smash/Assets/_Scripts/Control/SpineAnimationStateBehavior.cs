using UnityEngine;
using Spine;
using Spine.Unity;
using PSmash.Movement;

namespace PSmash.Control
{

    public class SpineAnimationStateBehavior : StateMachineBehaviour
    {
        [SerializeField] AnimationReferenceAsset animation = null;
        [SerializeField] string parameter = null;
        [SerializeField] bool loop = false;
        [SerializeField] bool animSpeedModifiable = false;
        SkeletonAnimation spine = null;


        // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            spine = animator.GetComponentInChildren<SkeletonAnimation>();
            TrackEntry entry = spine.AnimationState.SetAnimation(0, animation, loop);
            //float speedModifier = GetAnimSpeedModifier(animator, ref stateInfo);
            entry.TimeScale = stateInfo.speed * stateInfo.speedMultiplier;
            spine.loop = loop;

            Debug.Log(entry.TimeScale + "  " + stateInfo.speed);

        }

        //private float GetAnimSpeedModifier(Animator animator, ref AnimatorStateInfo stateInfo)
        //{
        //    float speedModifier;
        //    if (animSpeedModifiable)
        //    {
        //        animator.s
        //        speedModifier = stateInfo.speed * animator.GetComponent<EnemyMovement>().GetAnimSpeedModifier();
        //    }
        //    else
        //    {
        //        speedModifier = stateInfo.speedMultiplier;
        //    }
        //    return speedModifier;
        //}

        // OnStateUpdate is called before OnStateUpdate is called on any state inside this state machine
        //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateExit is called before OnStateExit is called on any state inside this state machine
        //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateMove is called before OnStateMove is called on any state inside this state machine
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

        // OnStateIK is called before OnStateIK is called on any state inside this state machine
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    
        //}

    }
}


