using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_Grinding : State
{
    private Player player;
    private PlayerStateMachine stateMachine;

    public PlayerState_Grinding(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.player = stateMachine.player;
    }

    public override void Enter()
    {
    }

    public override void Update()
    {
        if (player.curGrindable == null)
        {
            Debug.LogWarning("got into grinding state with no grindable");
            stateMachine.ChangeState(stateMachine.airborneState);
            return;
        }

        Vector3 curVelocity = Vector3.ProjectOnPlane(player.velocity, Vector3.up);
        Vector3 aToB = (player.curGrindable.pointB.position - player.curGrindable.pointA.position).normalized;
        Vector3 bToA = (player.curGrindable.pointA.position - player.curGrindable.pointB.position).normalized;

        float aToBDotProduct = Vector3.Dot(curVelocity, aToB);
        float bToADotProduct = Vector3.Dot(curVelocity, bToA);

        bool isAToB = aToBDotProduct > bToADotProduct;

        if (isAToB)
        {
            player.velocity = aToB * player.moveSpeed;
        }
        else
        {
            player.velocity = bToA * player.moveSpeed;
        }

        player.characterController.Move(player.velocity * Time.deltaTime);

        Grindable grindable = player.grindableDetector.GetGrindable();
        if (grindable == null)
        {
            stateMachine.ChangeState(stateMachine.airborneState);
            return;
        }
    }

    public override string GetName()
    {
        return "Grinding";
    }
}
