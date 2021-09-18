using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_Wipeout : State
{
    public static string NAME = "Wipeout";

    private Player player;
    private PlayerStateMachine stateMachine;

    public PlayerState_Wipeout(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.player = stateMachine.player;
    }

    public override string GetName()
    {
        return NAME;
    }

    public override void Enter()
    {
        player.playerInputActions.Player.Jump.performed += DoReset;

        Rigidbody headRigidBody = player.head.GetComponent<Rigidbody>();
        Rigidbody bodyRigidBody = player.body.GetComponent<Rigidbody>();

        headRigidBody.isKinematic = false;
        bodyRigidBody.isKinematic = false;
        player.body.GetComponent<Collider>().enabled = true;
        player.head.GetComponent<Collider>().enabled = true;

        headRigidBody.AddForce(player.velocity, ForceMode.VelocityChange);
        bodyRigidBody.AddForce(player.velocity, ForceMode.VelocityChange);


        FollowCamera followCamera = GameObject.FindObjectOfType<FollowCamera>();
        followCamera.follow = false;
        followCamera.lookAtTarget = player.head.transform;
    }

    public override void Exit()
    {
        player.playerInputActions.Player.Jump.performed -= DoReset;
    }

    private void DoReset(InputAction.CallbackContext context)
    {
        PlayerSpawner spawner = GameObject.FindObjectOfType<PlayerSpawner>();
        spawner.transform.position = player.transform.position + Vector3.up * 5f;
        Vector3 playerWorldVelocity = player.velocity;
        Vector3 parallelToGroundVelocity = Vector3.ProjectOnPlane(playerWorldVelocity, Vector3.up);
        Quaternion targetRotation = Quaternion.LookRotation(parallelToGroundVelocity, Vector3.up);
        spawner.transform.rotation = targetRotation;
        spawner.SpawnPlayer();

        player.gameObject.SetActive(false);
    }
}
