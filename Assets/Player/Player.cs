using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float crouchedSpeed = 10f;
    public float gravity = 0.1f;
    public float minJumpForce = 3f;
    public float maxJumpForce = 10f;
    public float timeToMaxJump = 1f;
    public float airborneMovementScalar = 0.3f;
    public int turnRate = 180; // deg/sec
    public int spinRate = 360; // deg/sec

    [SerializeField]
    public float groundAcceleration = 1f;

    [Range(0, 90)]
    public int leaveGrindOllieDegrees = 45;

    public bool alignWithGroundNormal = false;

    [Header("Stats")]
    public float wipeoutAngleThreshold = 45f;

    [Header("Movement State")]
    [SerializeField]
    public Vector3 velocity = new Vector3(0, 0, 0);
    public bool didLeaveVert = false;

    public PlayerInputActions playerInputActions;
    [HideInInspector]
    public InputAction movement;
    [HideInInspector]
    public InputAction cameraMovement;

    public PlayerStateMachine stateMachine;

    [HideInInspector]
    public PlayerController playerController;

    [SerializeField]
    private FreeLookUserInput freeLookUserInput;

    [HideInInspector]
    public GroundNormalDetector groundNormalDetector;

    [SerializeField]
    public GameObject body;

    [SerializeField]
    public GameObject head;

    [SerializeField]
    public GameObject playerPrefab;

    [SerializeField]
    public GrindableDetector grindableDetector;

    public Grindable curGrindable = null;
    public Transform grindTarget = null;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerController = GetComponent<PlayerController>();
        stateMachine = new PlayerStateMachine(this);
        groundNormalDetector = GetComponentInChildren<GroundNormalDetector>();
    }

    void OnEnable()
    {
        movement = playerInputActions.Player.Movement;
        movement.Enable();

        playerInputActions.Player.Jump.Enable();
        playerInputActions.Player.Grind.Enable();
    }

    void OnDisable()
    {
        movement.Disable();
        playerInputActions.Player.Jump.Disable();
        playerInputActions.Player.Grind.Disable();
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

    public PlayerStateMachine GetStateMachine()
    {
        return stateMachine;
    }
}
