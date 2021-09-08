using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_Grounded : State
{
    private Player player;
    private PlayerStateMachine stateMachine;

    public PlayerState_Grounded(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        player = stateMachine.player;
    }

    public override void Update()
    {
        Vector2 inputDir = player.movement.ReadValue<Vector2>();
        Vector3 desiredMoveDirectionGlobal = player.ConvertRelativeInputDirectionToWorldSpace(inputDir);

        Vector3 velocity = desiredMoveDirectionGlobal * player.moveSpeed;
        player.velocity.x = velocity.x;
        player.velocity.y = 0f;
        player.velocity.z = velocity.z;

        player.characterController.Move(player.velocity * Time.deltaTime + Vector3.down);

        if (!player.characterController.isGrounded)
        {
            player.velocity.y = 0f;
            stateMachine.ChangeState(stateMachine.airborneState);
            return;
        }

        AlignHeading();
    }

    public override void Enter()
    {
        player.playerInputActions.Player.Jump.performed += DoJump;
    }

    public override void Exit()
    {
        player.playerInputActions.Player.Jump.performed -= DoJump;
    }

    public override string GetName()
    {
        return "Grounded";
    }

    private void DoJump(InputAction.CallbackContext context)
    {
        player.velocity.y = player.jumpForce;
        player.characterController.Move(player.velocity * Time.deltaTime);
        stateMachine.ChangeState(stateMachine.airborneState);
    }

    private void AlignHeading()
    {
        Vector3? maybeGroundNormal = player.groundNormalDetector.GetGroundNormal();
        if (maybeGroundNormal.HasValue)
        {
            Vector3 groundNormal = maybeGroundNormal.Value;
            if (player.velocity.magnitude > 0.01f)
            {
                //Make sure the velocity is normalized
                Vector3 vel = player.velocity.normalized;
                //Project the two vectors using the dot product
                Vector3 forward = vel - groundNormal * Vector3.Dot(vel, groundNormal);
                //Set the rotation with relative forward and up axes
                player.transform.rotation = Quaternion.LookRotation(forward.normalized, groundNormal);
            }
            else
            {
                //Make sure the velocity is normalized
                Vector3 vel = player.transform.forward;
                //Project the two vectors using the dot product
                Vector3 forward = vel - groundNormal * Vector3.Dot(vel, groundNormal);
                //Set the rotation with relative forward and up axes
                player.transform.rotation = Quaternion.LookRotation(forward.normalized, groundNormal);
            }
        }
        else if (player.velocity.magnitude > 0.01f)
        {
            player.transform.rotation = Quaternion.LookRotation(player.velocity.normalized, Vector3.up);
        }
    }
}
