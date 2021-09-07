using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_Grounded : State
{
    private Player player;
    private PlayerStateMachine stateMachine;

    public PlayerState_Grounded(PlayerStateMachine stateMachine) {
        this.stateMachine = stateMachine;
        player = stateMachine.player;
    }

    public override void Update() {
        Vector2 inputDir = player.movement.ReadValue<Vector2>();

        float cameraFacing = player.GetCameraFacing();
        Vector3 desiredMoveDirectionLocal = new Vector3(inputDir.x, 0, inputDir.y);
        Vector3 desiredMoveDirectionGlobal = Quaternion.Euler(0, cameraFacing, 0) * desiredMoveDirectionLocal;

        Vector3 velocity = desiredMoveDirectionGlobal * player.moveSpeed;
        player.velocity.x = velocity.x;
        player.velocity.y = 0f;
        player.velocity.z = velocity.z;

        player.characterController.Move(player.velocity * Time.deltaTime + Vector3.down);

        if (player.velocity.x != 0 || player.velocity.z != 0) {
            Quaternion rotation = Quaternion.LookRotation(player.velocity);
            player.transform.rotation = rotation;
        }

        if (!player.characterController.isGrounded) {
            player.velocity.y = 0f;
            stateMachine.ChangeState(stateMachine.airborneState);
        }
    }

    public override void Enter() {
        player.playerInputActions.Player.Jump.performed += DoJump;
    }
    
    public override void Exit() {
        player.playerInputActions.Player.Jump.performed -= DoJump;
    }

    public override string GetName() {
        return "Grounded";
    }

    private void DoJump(InputAction.CallbackContext context) {
        player.velocity.y = player.jumpForce;
        player.characterController.Move(player.velocity * Time.deltaTime);
        stateMachine.ChangeState(stateMachine.airborneState);
    }
}
