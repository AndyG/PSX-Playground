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
        Vector3 moveVector = new Vector3(0, 0, 0);
        moveVector.y = -player.gravity;
        player.characterController.Move(moveVector * Time.deltaTime);
        if (player.characterController.isGrounded) {
            stateMachine.ChangeState(stateMachine.groundedState);
        }
    }

    public override string GetName() {
        return "Airborne";
    }
}
