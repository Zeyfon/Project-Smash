using PSmash.Control;

namespace PSmash.InputSystem
{
    public class ToolCommand : ActionCommand, ICommand
    {
        PlayerController playerController;

        void Start()
        {
            playerController = transform.parent.GetComponent<PlayerController>();
        }
        void ICommand.Execute(bool isButtonPressed)
        {
            Attack(isButtonPressed);
        }

        void Attack(bool isPressedAttack)
        {
            playerController.ToolButton(isPressedAttack);
        }
    }
}

