using UnityEngine;
using Spine;
using Spine.Unity;

namespace PSmash.Control
{

    public class AnimationBehavior : StateMachineBehaviour
    {
        [SerializeField] AnimationReferenceAsset animation = null;
        [SerializeField] string parameter = null;
        [SerializeField] bool loop = false;
        SkeletonAnimation spine = null;

        // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            spine = animator.GetComponentInChildren<SkeletonAnimation>();
            TrackEntry entry = spine.AnimationState.SetAnimation(0, animation, loop);

            entry.TimeScale = animator.speed * stateInfo.speedMultiplier;
            spine.loop = loop;
            if (parameter != "")
                animator.SetInteger(parameter, animator.GetInteger(parameter) + 1);
        }

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

        // OnStateMachineEnter is called when entering a state machine via its Entry Node
        override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            animator.SetInteger(parameter, animator.GetInteger(parameter) + 1);
        }

        // OnStateMachineExit is called when exiting a state machine via its Exit Node
        override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            animator.SetInteger(parameter, 100);
            //Debug.Break();
            //print(action + " set to 0");
            //Next lines are for PlayMaker Use only
            //PlayMakerFSM fsm = animator.gameObject.GetComponent<PlayMakerFSM>();
            //if(fsm != null) 
            //    animator.gameObject.GetComponent<PlayMakerFSM>().   .SendEvent("ANIMATIONFINISHED");
        }
    }
}


