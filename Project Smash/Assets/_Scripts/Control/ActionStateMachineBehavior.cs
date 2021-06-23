using UnityEngine;

namespace PSmash.Control
{
    /// <summary>
    /// In Charge of the incremental int for the animation tracking
    /// </summary>
    public class ActionStateMachineBehavior : StateMachineBehaviour
    {
        [SerializeField] string action = null;

        // OnStateEnter is called before OnStateEnter is called on any state inside this state machine
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            //Debug.Log("Entered State");
            animator.SetInteger(action, animator.GetInteger(action) + 1);
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
            //Debug.Log("Entered State Machine");
            animator.SetInteger(action, animator.GetInteger(action) + 1);
        }

        // OnStateMachineExit is called when exiting a state machine via its Exit Node
        override public void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            animator.SetInteger(action, 100);
            //Debug.Log(animator.gameObject + "  Exited Animation");
        }
    }
}

