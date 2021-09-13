using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState_Airborne : State
{
    private Player player;
    private PlayerStateMachine stateMachine;

    public PlayerState_Airborne(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.player = stateMachine.player;
    }

    public override void Update()
    {
        if (player.characterController.isGrounded)
        {
            stateMachine.ChangeState(stateMachine.groundedState);
            return;
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
}
