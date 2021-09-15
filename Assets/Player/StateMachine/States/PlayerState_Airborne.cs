using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_Airborne : State
{
    private Player player;
    private PlayerStateMachine stateMachine;

    private bool isLookingForGrind = false;

    public PlayerState_Airborne(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.player = stateMachine.player;
    }

    public override void Update()
    {
        if (player.characterController.isGrounded)
        {
            if (LandingShouldWipeout())
            {
                stateMachine.ChangeState(stateMachine.wipeoutState);
                return;
            }
            else
            {
                Vector3 velocityRelativeToGround = Vector3.ProjectOnPlane(player.velocity, Vector3.up);
                player.transform.LookAt(player.transform.position + velocityRelativeToGround, player.transform.up);
                stateMachine.ChangeState(stateMachine.groundedState);
                return;
            }
        }

        if (isLookingForGrind)
        {
            Grindable grindable = player.grindableDetector.GetGrindable();
            if (grindable != null)
            {
                player.curGrindable = grindable;
                stateMachine.ChangeState(stateMachine.grindingState);
                return;
            }
        }

        Vector2 inputDir = player.movement.ReadValue<Vector2>();
        float degsToRotate = inputDir.x * player.spinRate * Time.deltaTime;
        player.transform.Rotate(new Vector3(0, degsToRotate, 0), Space.Self);

        // apply gravity
        player.velocity.y -= player.gravity * Time.deltaTime;
        player.characterController.Move(player.velocity * Time.deltaTime);
    }

    public override string GetName()
    {
        return "Airborne";
    }

    public override void Enter()
    {
        isLookingForGrind = false;
        player.playerInputActions.Player.Grind.started += OnGrindButtonPressed;
        player.playerInputActions.Player.Grind.canceled += OnGrindButtonReleased;
    }

    public override void Exit()
    {
        isLookingForGrind = false;
        player.playerInputActions.Player.Grind.started -= OnGrindButtonPressed;
        player.playerInputActions.Player.Grind.canceled -= OnGrindButtonReleased;
    }

    private void OnGrindButtonPressed(InputAction.CallbackContext context)
    {
        isLookingForGrind = true;
    }

    private void OnGrindButtonReleased(InputAction.CallbackContext context)
    {
        isLookingForGrind = false;
    }

    private bool LandingShouldWipeout()
    {
        Vector3 velocityRelativeToGround = Vector3.ProjectOnPlane(player.velocity, Vector3.up);
        if (velocityRelativeToGround.magnitude < 3f)
        {
            return false;
        }

        Quaternion velocityOrientation = Quaternion.LookRotation(velocityRelativeToGround, Vector3.up);

        Quaternion lookAngle = Quaternion.Euler(0, player.transform.rotation.eulerAngles.y, 0);
        float oppositeLookAngleDeg = lookAngle.eulerAngles.y + 180;
        if (oppositeLookAngleDeg > 360)
        {
            oppositeLookAngleDeg = oppositeLookAngleDeg - 360;
        }

        Quaternion oppositeLookAngle = Quaternion.Euler(0, oppositeLookAngleDeg, 0);

        float angleForward = Mathf.Abs(Quaternion.Angle(lookAngle, velocityOrientation));
        float angleBackward = Mathf.Abs(Quaternion.Angle(oppositeLookAngle, velocityOrientation));

        return angleForward > player.wipeoutAngleThreshold && angleBackward > player.wipeoutAngleThreshold
        && angleForward < (180 - player.wipeoutAngleThreshold) && angleBackward < (180 - player.wipeoutAngleThreshold);
    }
}
