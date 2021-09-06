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
        Vector3 moveVector = new Vector3(inputDir.x, 0, inputDir.y) * player.moveSpeed;
        moveVector.y = -player.gravity;
        player.characterController.Move(moveVector * Time.deltaTime);
        if (!player.characterController.isGrounded) {
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
        Debug.Log("DoJump");
    }
}
