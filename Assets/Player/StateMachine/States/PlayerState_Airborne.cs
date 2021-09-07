using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Airborne : State
{
    private Player player;
    private PlayerStateMachine stateMachine;

    public PlayerState_Airborne(PlayerStateMachine stateMachine) {
        this.stateMachine = stateMachine;
        this.player = stateMachine.player;
    }

    public override void Update() {
        if (player.characterController.isGrounded) {
            stateMachine.ChangeState(stateMachine.groundedState);
            return;
        }

        Vector2 inputDir = player.movement.ReadValue<Vector2>();
        Vector2 scaledMovementVector = inputDir * player.airborneMovementScalar * player.moveSpeed;
        player.velocity.x = scaledMovementVector.x * Time.deltaTime;
        player.velocity.z = scaledMovementVector.y * Time.deltaTime;
        player.velocity.y -= player.gravity * Time.deltaTime;
        player.characterController.Move(player.velocity);
    }

    public override string GetName() {
        return "Airborne";
    }
}
