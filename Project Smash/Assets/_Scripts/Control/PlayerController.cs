using PSmash.Combat;
using PSmash.Movement;
using PSmash.Attributes;
using UnityEngine;

namespace PSmash.Control
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] GameObject spriteGameObject = null;
        PlayerMovement playerMovement;
        PlayerFighter fighter;
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
            playerMovement = GetComponent<PlayerMovement>();
            fighter = GetComponent<PlayerFighter>();
            health = GetComponent<PlayerHealth>();
        }

        private void OnEnable()
        {
            Destroy(spriteGameObject);
            xInput = 0;
            yInput = 0;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (!isEnabled) return;
            if (fighter.IsAttacking() || fighter.IsFinishingAnEnemy()|| health.IsDamaged() || health.IsDead()) return;
            InteractWithMovement();
        }

        private void InteractWithMovement()
        {
            //playerMovement.ControlledMovement(xInput, yInput, false);
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
            //playerMovement.ControlledMovement(xInput, yInput, true);
            //playerMovement.Jump(yInput);
        }

        public void MainAttackButton(bool isButtonPressed)
        {
            if (!isEnabled) return;
            if (playerMovement.IsEvading() || playerMovement.IsMovingOnWall || playerMovement.IsClimbingLedge() || playerMovement.IsMovingOnLadder() || health.IsDamaged()) return;
            fighter.MainAttack(isButtonPressed, yInput);
        }

        public void ToolButton(bool isButtonPressed)
        {
            if (!isEnabled) return;
            if (playerMovement.IsEvading() || playerMovement.IsMovingOnWall || playerMovement.IsClimbingLedge() || playerMovement.IsMovingOnLadder() || health.IsDamaged()) return;
            if (ToolAction()) return;
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

        public void ThrowButton(bool isButtonPressed)
        {
            if (!isEnabled) return;
            //print("Attacking");
            if (fighter.IsAttacking() || playerMovement.IsEvading() || playerMovement.IsMovingOnWall || playerMovement.IsClimbingLedge() || playerMovement.IsMovingOnLadder() || health.IsDamaged()) return;
            fighter.ThrowItemAttack(isButtonPressed);
        }

        bool ToolAction()
        {
            if (fighter.IsAttacking()) return false;
            if (fighter.ToolAttack()) return false;

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
            //playerMovement.ControlledMovement(0, 0, false);
            this.enabled = state;
            isEnabled = state;
        }


    }
}
