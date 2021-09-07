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
        player = stateMachine.player;
        Vector2 inputDir = player.movement.ReadValue<Vector2>();
        player.velocity.x = inputDir.x * player.moveSpeed;
        player.velocity.z = inputDir.y * player.moveSpeed;
        player.velocity.y = -1f;
        player.characterController.Move(player.velocity * Time.deltaTime);

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
