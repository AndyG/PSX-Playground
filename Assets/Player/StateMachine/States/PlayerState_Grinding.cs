using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_Grinding : State
{
    public const string NAME = "Grinding";
    private Player player;
    private PlayerStateMachine stateMachine;

    private Transform target;

    private float timeSpentHoldingDirection = 0f;
    private bool isCrouched = false;
    private float timeSpentCrouched = 0f;

    public PlayerState_Grinding(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.player = stateMachine.player;
    }

    public override void Enter()
    {
        ResetState();

        Grindable grindable = player.curGrindable;
        Collider grindableCollider = grindable.GetComponent<Collider>();
        Vector3 targetPosition = grindableCollider.ClosestPoint(player.grindableDetector.transform.position);
        player.transform.position = targetPosition + Vector3.up;

        Vector3 curVelocity = Vector3.ProjectOnPlane(player.velocity, Vector3.up);
        Vector3 aToB = Vector3.ProjectOnPlane(player.curGrindable.pointB.position - player.curGrindable.pointA.position, Vector3.up).normalized;
        Vector3 bToA = Vector3.ProjectOnPlane(player.curGrindable.pointA.position - player.curGrindable.pointB.position, Vector3.up).normalized;

        float aToBDotProduct = Vector3.Dot(curVelocity, aToB);
        float bToADotProduct = Vector3.Dot(curVelocity, bToA);

        bool isAToB = aToBDotProduct > bToADotProduct;

        if (isAToB)
        {
            target = grindable.pointB.transform;
        }
        else
        {
            target = grindable.pointA.transform;
        }

        EnableJumpInput();
    }

    public override void Exit()
    {
        ResetState();
        DisableJumpInput();

        player.playerInputActions.Player.Jump.started -= StartCrouch;
        player.playerInputActions.Player.Jump.canceled -= OnCrouchRelease;
    }

    public override void Update()
    {
        float playerInputHoriz = Math.Sign(player.movement.ReadValue<Vector2>().x);
        if (playerInputHoriz != 0)
        {
            timeSpentHoldingDirection += Time.deltaTime;
        }
        else
        {
            timeSpentHoldingDirection = 0f;
        }

        Vector3 direction = (target.position - player.grindableDetector.transform.position).normalized;

        float distanceToMove = (direction * player.moveSpeed * Time.deltaTime).magnitude;
        float distanceToTarget = Vector3.Distance(target.position, player.grindableDetector.transform.position);

        player.velocity = direction * player.moveSpeed;

        if (distanceToMove > distanceToTarget)
        {
            player.characterController.Move(direction * distanceToTarget);
            GrindableGroup group = player.curGrindable.GetGroup();
            if (group == null)
            {
                EndGrind();
                return;
            }

            Grindable nextGrindable = group.GetNextGrindable(player.curGrindable, player.grindableDetector.transform);
            if (nextGrindable == null)
            {
                EndGrind();
                return;
            }

            TransferToGrindable(nextGrindable);
        }
        else
        {
            player.characterController.Move(player.velocity * Time.deltaTime);
        }
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
        int jumpOffSide = 0;
        // compute new velocity in the x/z plane
        if (timeSpentHoldingDirection > 0.25f)
        {
            float playerInputHoriz = player.movement.ReadValue<Vector2>().x;
            jumpOffSide = Math.Sign(playerInputHoriz);
        }

        Vector3 curHeading = Vector3.ProjectOnPlane(player.velocity, Vector3.up).normalized;
        Quaternion rotationQuaternion = Quaternion.Euler(0, player.leaveGrindOllieDegrees * jumpOffSide, 0);
        Vector3 newHeading = rotationQuaternion * curHeading;

        float jumpHeldFraction = Mathf.Min(timeSpentCrouched / player.timeToMaxJump, 1f);
        float verticalVelocityForJump = player.velocity.y + Mathf.Max(player.minJumpForce, player.maxJumpForce * jumpHeldFraction);

        player.velocity = new Vector3(newHeading.x, 0, newHeading.z) * player.moveSpeed + Vector3.up * verticalVelocityForJump;
        stateMachine.ChangeState(stateMachine.airborneState);
    }

    private void ResetState()
    {
        timeSpentHoldingDirection = 0f;
        isCrouched = false;
        timeSpentCrouched = 0f;
    }

    private void TransferToGrindable(Grindable grindable)
    {
        // transfer to next grindable
        player.curGrindable = grindable;
        Transform nextGrindablePointA = grindable.pointA;
        Transform nextGrindablePointB = grindable.pointB;
        float distanceToNextPointA = Vector3.Distance(player.grindableDetector.transform.position, nextGrindablePointA.position);
        float distanceToNextPointB = Vector3.Distance(player.grindableDetector.transform.position, nextGrindablePointB.position);
        bool isNextTargetPointA = distanceToNextPointA > distanceToNextPointB;
        if (isNextTargetPointA)
        {
            this.target = nextGrindablePointA;
        }
        else
        {
            this.target = nextGrindablePointB;
        }
    }

    private void EndGrind()
    {
        stateMachine.ChangeState(stateMachine.airborneState);
    }

    private void EnableJumpInput()
    {
        player.playerInputActions.Player.Jump.started += StartCrouch;
        player.playerInputActions.Player.Jump.canceled += OnCrouchRelease;
    }

    private void DisableJumpInput()
    {
        player.playerInputActions.Player.Jump.started -= StartCrouch;
        player.playerInputActions.Player.Jump.canceled -= OnCrouchRelease;
    }

    public override string GetName()
    {
        return NAME;
    }
}
