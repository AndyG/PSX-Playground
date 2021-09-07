using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float turnSpeed = 1f;
    public float minTurnAngle = -90.0f;
    public float maxTurnAngle = 90.0f;
    public float gravity = 0.1f;
    public float jumpForce = 10f;
    public float airborneMovementScalar = 0.3f;

    [Header("Movement State")]
    public Vector3 velocity = new Vector3(0, 0, 0);

    public PlayerInputActions playerInputActions;
    [HideInInspector]
    public InputAction movement;
    public PlayerStateMachine stateMachine;

    [HideInInspector]
    public CharacterController characterController;

    void Awake() {
        playerInputActions = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();
        stateMachine = new PlayerStateMachine(this);
    }

    void OnEnable() {
        movement = playerInputActions.Player.Movement;
        movement.Enable();
        playerInputActions.Player.Jump.Enable();
    }

    void OnDisable() {
        movement.Disable();
        playerInputActions.Player.Jump.Disable();
    }

    void Start()
    {
        stateMachine.Start();
    }

    void Update()
    {
        stateMachine.Update();
    }

    void OnGUI()
    {
        stateMachine.OnGUI();
    }

    protected virtual State GetInitialState()
    {
        return null;
    }
}
