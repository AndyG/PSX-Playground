using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_Grounded : State
{
    public const string NAME = "Grounded";
    private Player player;
    private PlayerStateMachine stateMachine;

    private bool isCrouched = false;
    private float timeSpentCrouched = 0f;

    public PlayerState_Grounded(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        player = stateMachine.player;
    }

    public override void Update()
    {
        if (isCrouched)
        {
            timeSpentCrouched += Time.deltaTime;
        }

        // turn player
        Vector2 inputDir = player.movement.ReadValue<Vector2>();
        if (Mathf.Abs(inputDir.x) > 0.1)
        {
            float rotationAmount = player.turnRate * Math.Sign(inputDir.x) * Time.deltaTime;
            player.transform.Rotate(Vector3.up * rotationAmount, Space.Self);
        }

        bool isHoldingBack = inputDir.y < -0.5f;
        if (isHoldingBack)
        {
            return;
        }

        float speed = isCrouched ? player.crouchedSpeed : player.moveSpeed;
        Vector3 desiredVelocity = player.transform.forward.normalized * speed;
        player.velocity = desiredVelocity;

        player.playerController.Move(player.velocity * Time.deltaTime);

        AlignHeadingWithGroundNormal();

        if (!player.playerController.IsGrounded() && !player.groundNormalDetector.GetGroundNormal().HasValue)
        {
            stateMachine.ChangeState(stateMachine.airborneState);
            return;
        }
    }

    public override void Enter()
    {
        ResetState();

        player.playerInputActions.Player.Jump.started += StartCrouch;
        player.playerInputActions.Player.Jump.canceled += OnCrouchRelease;

        InputActionPhase jumpPhase = player.playerInputActions.Player.Jump.phase;
        if (jumpPhase == InputActionPhase.Started || jumpPhase == InputActionPhase.Performed)
        {
            StartCrouch();
        }
    }

    public override void Exit()
    {
        ResetState();

        player.playerInputActions.Player.Jump.started -= StartCrouch;
        player.playerInputActions.Player.Jump.canceled -= OnCrouchRelease;
    }

    public override string GetName()
    {
        return NAME;
    }

    private void StartCrouch(InputAction.CallbackContext context)
    {
        StartCrouch();
    }

    private void StartCrouch()
    {
        isCrouched = true;
    }

    private void OnCrouchRelease(InputAction.CallbackContext context)
    {
        float fraction = Mathf.Min(timeSpentCrouched / player.timeToMaxJump, 1f);
        player.velocity.y = Mathf.Max(player.minJumpForce, player.maxJumpForce * fraction);
        stateMachine.ChangeState(stateMachine.airborneState);
    }

    private void ResetState()
    {
        isCrouched = false;
        timeSpentCrouched = 0f;
    }

    private void AlignHeadingWithGroundNormal()
    {
        if (!player.alignWithGroundNormal) return;

        Vector3? maybeGroundNormal = player.groundNormalDetector.GetGroundNormal();
        if (maybeGroundNormal.HasValue)
        {
            Vector3 forward = player.transform.forward.normalized;
            Vector3 newUp = maybeGroundNormal.Value.normalized;
            Vector3 left = Vector3.Cross(forward, newUp);
            Vector3 newForward = Vector3.Cross(newUp, left);
            Quaternion newRotation = Quaternion.LookRotation(newForward, newUp);
            player.transform.rotation = newRotation;
        }
    }
}
