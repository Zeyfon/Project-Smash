using PSmash.Combat;
using PSmash.Movement;
using PSmash.Resources;
using UnityEngine;

namespace PSmash.Control
{
    public class PlayerControllerV2 : MonoBehaviour
    {
        //public static Transform playerTransform;
        PlayerMovementV2 playerMovement;
        PlayerFighterV2 fighter;
        PlayerHealth health;
        ToolSystem tools;
        ToolActionList toolAction;
        bool isInteractingWithObject = false;
        bool isEnabled = true;
        float xInput;
        float yInput;

        private void Awake()
        {
            tools = GetComponent<ToolSystem>();
            playerMovement = GetComponent<PlayerMovementV2>();
            fighter = GetComponent<PlayerFighterV2>();
            health = GetComponent<PlayerHealth>();
        }

        private void OnEnable()
        {
            xInput = 0;
            yInput = 0;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!isEnabled) return;
            if (fighter.IsAttacking() || health.IsDamaged() || health.IsDead()) return;
            InteractWithMovement();
        }

        private void InteractWithMovement()
        {
            playerMovement.ControlledMovement(xInput, yInput, isInteractingWithObject);
        }


        public void GetMovement(float xInput, float yInput)
        {
            this.xInput = xInput;
            this.yInput = yInput;
        }

        //Action Buttons
        public void JumpButtonPressed()
        {
            if (!isEnabled) return;
            //print("Jump");
            if (fighter.IsAttacking() || playerMovement.IsEvading() || playerMovement.IsClimbingLedge() || fighter.IsGuarding() || health.IsDamaged()) return;
            playerMovement.Jump(yInput);
        }

        public void MainAttackButton(bool isButtonPressed)
        {
            if (!isEnabled) return;
            //print("Attacking");
            if (playerMovement.IsEvading() || playerMovement.IsMovingOnWall || playerMovement.IsClimbingLedge() || playerMovement.IsMovingOnLadder() || health.IsDamaged()) return;
            fighter.MainAttack(isButtonPressed, yInput);
        }

        public void ToolButton(bool isButtonPressed)
        {
            if (!isEnabled) return;
            if (playerMovement.IsEvading() || playerMovement.IsMovingOnWall || playerMovement.IsClimbingLedge() || playerMovement.IsMovingOnLadder() || health.IsDamaged()) return;
            if (CheckToolAction()) return;
            //fighter.ToolAttack(isButtonPressed);
        }

        public void GuardButton(bool isButtonPressed)
        {
            if (!isEnabled) return;
            if (fighter.IsAttacking() || playerMovement.IsEvading() || playerMovement.IsClimbingLedge() || playerMovement.IsMovingOnWall || playerMovement.IsMovingOnLadder() || health.IsDamaged()) return;
            fighter.Guard(isButtonPressed);
        }

        public void EvadeButton()
        {
            if (!isEnabled) return;
            if (fighter.IsAttacking() || playerMovement.IsEvading() || playerMovement.IsClimbingLedge() || playerMovement.IsMovingOnWall || playerMovement.IsMovingOnLadder() || fighter.IsGuarding() || health.IsDamaged()) return;
            playerMovement.EvadeMovement(xInput);
        }

        bool CheckToolAction()
        {
            if (fighter.IsAttacking()) return false;
            //print("CheckingTool Action");

            switch (toolAction)
            {
                case ToolActionList.WallMovement:
                    playerMovement.IsMovingOnWall = true;
                    return true;
                default:
                    return false;
            }
        }

        public void CurrentToolAction(ToolActionList toolAction)
        {
            this.toolAction = toolAction;
            //print(toolAction);
        }
        public void StopWallMovement()
        {
            print("Stopping wall");
            playerMovement.IsMovingOnWall = false;
        }

        public void SetEnable(bool state)
        {
            //print("Playercontroller Disabled");
            playerMovement.ControlledMovement(0, 0, false);
            this.enabled = state;
            isEnabled = state;
        }


    }
}
