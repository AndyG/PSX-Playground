using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float gravity = 0.1f;
    public float jumpForce = 10f;
    public float airborneMovementScalar = 0.3f;
    public int turnRate = 180; // deg/sec
    public int spinRate = 360; // deg/sec

    [Header("Movement State")]
    public Vector3 velocity = new Vector3(0, 0, 0);

    public PlayerInputActions playerInputActions;
    [HideInInspector]
    public InputAction movement;
    [HideInInspector]
    public InputAction cameraMovement;

    public PlayerStateMachine stateMachine;

    [HideInInspector]
    public CharacterController characterController;

    [SerializeField]
    private FreeLookUserInput freeLookUserInput;

    [HideInInspector]
    public GroundNormalDetector groundNormalDetector;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();
        characterController = GetComponent<CharacterController>();
        stateMachine = new PlayerStateMachine(this);
        groundNormalDetector = GetComponentInChildren<GroundNormalDetector>();
    }

    void OnEnable()
    {
        movement = playerInputActions.Player.Movement;
        movement.Enable();

        playerInputActions.Player.Jump.Enable();

    }

    void OnDisable()
    {
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

    public void LookAtDirection(Vector3 direction, bool parallelToGround = true)
    {
        Quaternion rotation;
        if (parallelToGround)
        {
            rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(direction, Vector3.up));
        }
        else
        {
            rotation = Quaternion.LookRotation(direction);
        }
        transform.rotation = rotation;
    }

    protected virtual State GetInitialState()
    {
        return null;
    }
}
