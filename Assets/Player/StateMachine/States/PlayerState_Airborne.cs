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
        Vector3 desiredMoveDirectionGlobal = player.ConvertRelativeInputDirectionToWorldSpace(inputDir);

        Vector3 velocity = desiredMoveDirectionGlobal * player.moveSpeed * player.airborneMovementScalar;
        player.velocity.x = velocity.x * Time.deltaTime;
        player.velocity.y -= player.gravity * Time.deltaTime;
        player.velocity.z = velocity.z * Time.deltaTime;

        player.characterController.Move(player.velocity);

        if (player.velocity.x != 0 || player.velocity.z != 0) {
            player.LookAtDirection(player.velocity);
        }
    }

    public override string GetName() {
        return "Airborne";
    }
}
