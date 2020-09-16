using PSmash.Control;

namespace PSmash.InputSystem
{
    public class HeavyAttackCommand : ActionCommand, ICommandV2
    {
        PlayerControllerV2 playerController;

        void Start()
        {
            playerController = transform.parent.GetComponent<PlayerControllerV2>();
        }
        void ICommandV2.Execute(bool isButtonPressed)
        {
            Attack(isButtonPressed);
        }

        void Attack(bool isPressedAttack)
        {
            playerController.ToolButton(isPressedAttack);
        }
    }
}

