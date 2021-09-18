using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState_Airborne : State
{
    public const string NAME = "Airborne";
    private Player player;
    private PlayerStateMachine stateMachine;

    public PlayerState_Airborne(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
        this.player = stateMachine.player;
    }

    public override void Update()
    {
        if (player.playerController.IsGrounded() && player.velocity.y <= 0)
        {
            if (LandingShouldWipeout())
            {
                stateMachine.ChangeState(stateMachine.wipeoutState);
                return;
            }
            else
            {
                Vector3 velocityRelativeToGround = Vector3.ProjectOnPlane(player.velocity, player.playerController.GetGroundNormal().Value);
                player.transform.LookAt(player.transform.position + velocityRelativeToGround, player.transform.up);
                stateMachine.ChangeState(stateMachine.groundedState);
                return;
            }
        }

        if (player.playerInputActions.Player.Grind.IsPressed())
        {
            Grindable grindable = player.grindableDetector.GetGrindable();
            if (grindable != null)
            {
                player.curGrindable = grindable;
                stateMachine.ChangeState(stateMachine.grindingState);
                return;
            }
        }

        Vector2 inputDir = player.movement.ReadValue<Vector2>();
        float degsToRotate = inputDir.x * player.spinRate * Time.deltaTime;
        player.transform.Rotate(new Vector3(0, degsToRotate, 0), Space.Self);

        // apply gravity
        player.velocity.y -= player.gravity * Time.deltaTime;
        player.playerController.Move(player.velocity * Time.deltaTime);
    }

    public override string GetName()
    {
        return NAME;
    }

    private bool LandingShouldWipeout()
    {
        Vector3? groundNormal = player.playerController.GetGroundNormal();
        if (!groundNormal.HasValue)
        {
            return false;
        }

        Vector3 velocityRelativeToGround = Vector3.ProjectOnPlane(player.velocity, groundNormal.Value);
        if (velocityRelativeToGround.magnitude < 3f)
        {
            return false;
        }

        // this is hack -- if player is straight up and the ground normal is horizontal-ish, don't wipe out
        bool isUpright = player.transform.up.y > 0.8f;
        bool isGroundNormalHorizontal = groundNormal.Value.y < 0.6;
        if (isUpright && isGroundNormalHorizontal)
        {
            Debug.Log("not wiping out");
            return false;
        }
        else
        {
            Debug.Log("was upright: " + isUpright + " ground normal y: " + groundNormal.Value.y);
        }

        Quaternion velocityOrientation = Quaternion.LookRotation(velocityRelativeToGround, groundNormal.Value);
        Quaternion playerRotation = player.transform.rotation;

        float similarity = Quaternion.Dot(velocityOrientation, playerRotation);
        float oppositeSimilarity = Quaternion.Dot(velocityOrientation, playerRotation * Quaternion.Euler(0, 180, 0));
        Debug.Log("checking similarity");
        return Mathf.Max(Mathf.Abs(similarity), Mathf.Abs(oppositeSimilarity)) < 0.9f;
    }
}
