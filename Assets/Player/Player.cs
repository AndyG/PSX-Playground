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

    [SerializeField]
    private new Camera camera;

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

        cameraMovement = playerInputActions.Player.CameraMovement;
        cameraMovement.Enable();

        playerInputActions.Player.Jump.Enable();

    }

    void OnDisable()
    {
        movement.Disable();
        cameraMovement.Disable();
        playerInputActions.Player.Jump.Disable();
    }

    void Start()
    {
        stateMachine.Start();
    }

    void Update()
    {
        stateMachine.Update();
        Vector2 inputDir = cameraMovement.ReadValue<Vector2>();
        freeLookUserInput.OnUserMovedCamera(inputDir);
    }

    void OnGUI()
    {
        stateMachine.OnGUI();
    }

    public float GetCameraFacing()
    {
        return camera.transform.eulerAngles.y;
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

    public Vector3 ConvertRelativeInputDirectionToWorldSpace(Vector2 inputDirection)
    {
        Vector3 inputDirectionRelative = new Vector3(inputDirection.x, 0, inputDirection.y);
        Vector3 inputDirectionWorld = Quaternion.Euler(0, GetCameraFacing(), 0) * inputDirectionRelative;
        return inputDirectionWorld;
    }

    protected virtual State GetInitialState()
    {
        return null;
    }
}
