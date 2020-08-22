using PSmash.Combat;
using PSmash.Movement;
using UnityEngine;

namespace PSmash.Control
{
    public class PlayerControllerV2 : MonoBehaviour
    {
        public static Transform playerTransform;
        PlayerMovementV2 playerMovement;
        PlayerFighterV2 fighter;
        ToolSystem tools;
        ToolActionList toolAction;
        bool isInteractingWithObject = false;
        float xInput;
        float yInput;

        private void Awake()
        {
            tools = GetComponent<ToolSystem>();
            playerMovement = GetComponent<PlayerMovementV2>();
            fighter = GetComponent<PlayerFighterV2>();
        }
        // Start is called before the first frame update
        void Start()
        {
            if (playerTransform) Destroy(gameObject);
            playerTransform = transform;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            if (fighter.IsAttacking || playerMovement.IsEvading) return;
            InteractWithMovement();
        }

        private void InteractWithMovement()
        {
            playerMovement.ControlledMovement(xInput, yInput, isInteractingWithObject);
        }


        public void GetMovement(float xInput, float yInput)
        {
            // The movement is set here by the InputHandler
            this.xInput = xInput;
            this.yInput = yInput;
        }

        //Action Buttons
        public void JumpButtonPressed()
        {
            if (fighter.IsAttacking || playerMovement.IsEvading || fighter.IsGuarding) return;
            playerMovement.Jump();
        }

        public void MainAttackButton(bool isButtonPressed)
        {
            if (playerMovement.IsEvading || playerMovement.IsMovingOnWall || playerMovement.IsMovingOnLadder) return;
           //fighter.MainAttack(isButtonPressed, yInput);
        }

        public void ToolButton(bool isButtonPressed)
        {
            if (playerMovement.IsEvading || playerMovement.IsMovingOnWall || playerMovement.IsMovingOnLadder) return;
            if (CheckToolAction()) return;
            //fighter.ToolAttack(isButtonPressed);
        }

        public void GuardButton(bool isButtonPressed)
        {
            Debug.Log("Parry button Pressed");
            if (fighter.IsAttacking ||playerMovement.IsEvading || playerMovement.IsMovingOnWall || playerMovement.IsMovingOnLadder) return;
            //Debug.Log("Will Do Defense Action");
            fighter.Guard(isButtonPressed);
        }

        public void EvadeButton()
        {
            if (fighter.IsAttacking || playerMovement.IsEvading || playerMovement.IsMovingOnWall || playerMovement.IsMovingOnLadder || fighter.IsGuarding) return;
            playerMovement.EvadeMovement(xInput);
        }

        bool CheckToolAction()
        {
            if (fighter.IsAttacking) return false;
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
    }
}
