using PSmash.Core;
using PSmash.Movement;
using PSmash.Saving;
using UnityEngine;
using HutongGames.PlayMaker;
using PSmash.Inventories;

namespace PSmash.InputSystem
{
    public abstract class Command
    {
        public abstract void ButtonPressed(Transform transform, InputHandler inputHandler, Equipment equipment, PlayMakerFSM pm, Vector2 input, float buttonState);

        public abstract void ButtonReleased(Transform transform, InputHandler inputHandler, PlayMakerFSM pm, Vector2 input, float buttonState);

    }

    public class JumpCommand : Command
    {
        LayerMask whatIsOneWayPlatform = LayerMask.GetMask("PlayerGround");
        public override void ButtonPressed(Transform transform, InputHandler inputHandler, Equipment equipment, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            IManualInteraction manualInteractableObject = transform.parent.GetComponent<InteractableElements>().GetInteractableObject();
            if (manualInteractableObject != null)
            {
                manualInteractableObject.Interact();
                inputHandler.EnableInput(false);
                return;
            }
            if (input.y < -0.5f)
            {
                RaycastHit2D hit = LookForOneWayPlatforms(transform);
                if (hit)
                {
                    RotateOneWayPlatform(hit);
                    return;
                }
            }
            pm.SendEvent("JUMPBUTTONPRESSED");
        }

        

        public override void ButtonReleased(Transform transform, InputHandler inputHandler, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            pm.SendEvent("JUMPBUTTONRELEASED");
        }
        RaycastHit2D LookForOneWayPlatforms(Transform transform)
        {
            Vector3 newPosition = transform.position + new Vector3(0, 0.5f, 0);
            RaycastHit2D hit = Physics2D.Raycast(newPosition, Vector2.down, 4, whatIsOneWayPlatform);
            return hit;
        }

        void RotateOneWayPlatform(RaycastHit2D hit)
        {
            hit.collider.GetComponent<OneWayPlatform>().RotatePlatform();
        }
    }

    public class EvadeCommand : Command
    {
        public override void ButtonPressed(Transform transform, InputHandler inputHandler, Equipment equipment, PlayMakerFSM pm, Vector2 input,float buttonState)
        {

            if (Mathf.Abs(input.x) > 0.4f)
            {
                pm.SendEvent("ROLLCOMMAND");
            }
            else
            {
                pm.SendEvent("BACKJUMPCOMMAND");
            }
        }

        public override void ButtonReleased(Transform transform, InputHandler inputHandler, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            //Do Nothing
        }
    }

    public class AttackCommand : Command
    {
        public override void ButtonPressed(Transform transform, InputHandler inputHandler, Equipment equipment, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            pm.SendEvent("MAINWEAPONATTACKCOMMAND");
        }

        public override void ButtonReleased(Transform transform, InputHandler inputHandler, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            //Do Nothing
        }
    }

    public class GuardCommand : Command
    {
        float prevButtonState = 0;
        //bool guardButton = false;
        FsmBool guardButtonState = FsmVariables.GlobalVariables.FindFsmBool("guardButtonState");
        public override void ButtonPressed(Transform transform, InputHandler inputHandler, Equipment equipment, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            if(buttonState==1 && buttonState != prevButtonState)
            {
                //Debug.Log("Button was pressed");
                pm.SendEvent("GUARDBUTTONPRESSED");
                guardButtonState.Value = true;
            }
            else if(buttonState ==0 && buttonState!= prevButtonState)
            {
                //Debug.Log("Button was released");
                pm.SendEvent("GUARDBUTTONRELEASED");
                guardButtonState.Value = false;
            }


            if (prevButtonState == 1)
            {
                //guardButtonState.Value = guardButton;
            }

            prevButtonState = buttonState;
            //pm.SendEvent("GUARDPRESSED");
        }

        public override void ButtonReleased(Transform transform, InputHandler inputHandler, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            //pm.SendEvent("GUARDRELEASED");
        }
    }

    public class SubweaponCommand : Command
    {
        public override void ButtonPressed(Transform transform, InputHandler inputHandler, Equipment equipment, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            pm.SendEvent("SUBWEAPONBUTTONPRESSED");
        }

        public override void ButtonReleased(Transform transform, InputHandler inputHandler, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            //pm.SendEvent("SUBWEAPONRELEASED");
        }
    }

    public class ToolCommand : Command
    {
        public override void ButtonPressed(Transform transform, InputHandler inputHandler, Equipment equipment, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            pm.SendEvent("ITEMBUTTONPRESSED");
        }

        public override void ButtonReleased(Transform transform, InputHandler inputHandler, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            //throw new System.NotImplementedException();
        }
    }

    public class SubweaponSwitchCommand : Command
    {
        public override void ButtonPressed(Transform transform, InputHandler inputHandler, Equipment equipment, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            equipment.SwitchSubWeapon();
        }

        public override void ButtonReleased(Transform transform, InputHandler inputHandler, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            //throw new System.NotImplementedException();
        }
    }

    public class ToolSelectionCommand : Command
    {
        float previousButtonState = 1;
        public override void ButtonPressed(Transform transform, InputHandler inputHandler, Equipment equipment, PlayMakerFSM pm, Vector2 input, float buttonState)
        {

            if(buttonState != previousButtonState && buttonState != 0)
            {
                transform.parent.GetComponent<Equipment>().UpdateCurrentEquippedTool(buttonState);

            }
            previousButtonState = buttonState;
        }

        public override void ButtonReleased(Transform transform, InputHandler inputHandler, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            
        }
    }

    public class MoveCommand : Command
    {
        FsmVector2 movementInput = FsmVariables.GlobalVariables.FindFsmVector2("movementInput");
        public override void ButtonPressed(Transform transform, InputHandler inputHandler, Equipment equipment, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            Debug.Log(input +"   here");
            movementInput.Value = input;
        }

        public override void ButtonReleased(Transform transform, InputHandler inputHandler, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            //throw new System.NotImplementedException();
        }
    }                

    public class GlideCommand : Command
    {
        public override void ButtonPressed(Transform transform, InputHandler inputHandler, Equipment equipment, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            Debug.Log(pm.Fsm.Name);
            pm.SendEvent("GLIDE");
        }

        public override void ButtonReleased(Transform transform, InputHandler inputHandler, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            //throw new System.NotImplementedException();
        }
    }

    public class UtilityCommand : Command
    {
        public override void ButtonPressed(Transform transform, InputHandler inputHandler, Equipment equipment, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            pm.SendEvent("UTILITYCOMMAND");
        }

        public override void ButtonReleased(Transform transform, InputHandler inputHandler, PlayMakerFSM pm, Vector2 input, float buttonState)
        {
            //throw new System.NotImplementedException();
        }
    }

}
