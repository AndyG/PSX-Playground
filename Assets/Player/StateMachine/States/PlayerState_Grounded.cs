using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_Grounded : State
{
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

        Vector2 inputDir = player.movement.ReadValue<Vector2>();
        // turn player
        Vector3 playerEulerAngles = player.transform.eulerAngles;
        playerEulerAngles.y += inputDir.x * player.turnRate * Time.deltaTime;
        player.transform.eulerAngles = playerEulerAngles;

        bool isHoldingBack = inputDir.y < -0.5f;
        if (isHoldingBack)
        {
            return;
        }

        Vector3 velocity = player.transform.forward * player.moveSpeed;
        player.velocity.x = velocity.x;
        player.velocity.y = 0f;
        player.velocity.z = velocity.z;

        Vector3 movement = velocity * Time.deltaTime + Vector3.down; // add gravity
        player.characterController.Move(movement);

        if (!player.characterController.isGrounded)
        {
            player.velocity.y = 0f;
            stateMachine.ChangeState(stateMachine.airborneState);
            return;
        }

        AlignHeading();
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
        return "Grounded";
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
        Debug.Log("time spent crouched: " + timeSpentCrouched);
        float fraction = Mathf.Min(timeSpentCrouched / player.timeToMaxJump, 1f);
        Debug.Log("fraction: " + fraction);
        player.velocity.y = Mathf.Max(player.minJumpForce, player.maxJumpForce * fraction);
        player.characterController.Move(player.velocity * Time.deltaTime);
        stateMachine.ChangeState(stateMachine.airborneState);
    }

    private void ResetState()
    {
        isCrouched = false;
        timeSpentCrouched = 0f;
    }

    private void AlignHeading()
    {
        Vector3? maybeGroundNormal = player.groundNormalDetector.GetGroundNormal();
        if (player.velocity.magnitude > 0.01f)
            if (maybeGroundNormal.HasValue)
            {
                Vector3 groundNormal = maybeGroundNormal.Value;
                //Make sure the velocity is normalized
                Vector3 vel = player.transform.forward.normalized;
                //Project the two vectors using the dot product
                Vector3 forward = vel - groundNormal * Vector3.Dot(vel, groundNormal);
                //Set the rotation with relative forward and up axes
                player.transform.rotation = Quaternion.LookRotation(forward.normalized, groundNormal);
            }
            else
            {
                player.transform.rotation = Quaternion.LookRotation(player.velocity.normalized, Vector3.up);
            }
    }
}
