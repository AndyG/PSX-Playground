using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [Header("Movement")]
    public float minSpeed = 5f;
    public float maxSpeed = 150f;
    public float coalesceSpeedStanding = 15f;
    public float coalesceSpeedCrouching = 30f;
    public float gravity = 0.1f;
    public float minJumpForce = 3f;
    public float maxJumpForce = 10f;
    public float timeToMaxJump = 1f;
    public float airborneMovementScalar = 0.3f;
    public int turnRate = 150; // deg/sec
    public int spinRate = 360; // deg/sec
    public float groundAccelerationStanding = 5f;
    public float groundAccelerationCrouching = 10f;
    [Range(-30f, 0f)]
    public float frictionEffectCrouching = -8f;
    [Range(-30f, 0f)]
    public float frictionEffectStanding = -16f;
    public float grindSpeed = 50f;
    public float vertLandingSpeedBoost = 100f;

    [Range(0, 90)]
    public int leaveGrindOllieDegrees = 45;

    public bool alignWithGroundNormal = false;

    [Header("Stats")]
    public float wipeoutAngleThreshold = 45f;

    [Header("Movement State")]
    [SerializeField]
    public Vector3 velocity = new Vector3(0, 0, 0);
    public float grindDisableTime = -1;

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

    private void OnGUI()
    {
        float speed = velocity.magnitude;
        string content = "Speed: " + speed;
        GUILayout.Label($"<color='white'><size=40>{content}</size></color>");
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
