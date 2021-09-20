using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float skinWidth = 0.02f;

    public float groundCheckRayDistance = 0.5f;
    public LayerMask terrainLayerMask;

    private Player player;
    private Collider playerCollider;

    private CharacterController characterController;

    public void Start()
    {
        player = GetComponent<Player>();
        playerCollider = player.GetComponent<CapsuleCollider>();
    }

    // return true if should wipe out
    public bool Move(Vector3 movement)
    {
        bool shouldWipeOut = false;
        if (!IsGrounded())
        {
            if (movement.y < 0f)
            {
                float distanceToGroundWorld = DistanceToGroundWorld(Mathf.Abs(movement.y));
                if (distanceToGroundWorld != -1 && distanceToGroundWorld < Mathf.Abs(movement.y))
                {
                    movement.y = -distanceToGroundWorld;
                    shouldWipeOut = LandingShouldWipeout(movement);
                }
            }
        }

        transform.position += movement;

        // overlapping colliders
        Collider[] overlappingColliders = Physics.OverlapCapsule(transform.position - transform.up * 0.5f, transform.position + transform.up * 0.5f, 0.5f, terrainLayerMask);
        Vector3 penetrationCorrectionDirection;
        float penetrationCorrectionDistance;
        foreach (Collider terrainCollider in overlappingColliders)
        {
            bool shouldCorrect = Physics.ComputePenetration(playerCollider, transform.position, transform.rotation, terrainCollider, terrainCollider.transform.position, terrainCollider.transform.rotation, out penetrationCorrectionDirection, out penetrationCorrectionDistance);
            if (shouldCorrect)
            {
                Vector3? groundNormal = GetGroundNormal();
                if (groundNormal.HasValue)
                {
                    AlignHeadingWithGroundNormal(groundNormal.Value);
                }
                else
                {
                    Vector3? groundNormalWorld = GetGroundNormalWorld();
                    if (groundNormal.HasValue)
                    {
                        AlignHeadingWithGroundNormal(groundNormal.Value);
                    }
                }

                transform.position += penetrationCorrectionDirection * penetrationCorrectionDistance;
            }
        }

        return shouldWipeOut;
    }

    public bool IsGrounded()
    {
        if (DistanceToGround(0.01f) != -1f)
        {
            Vector3? groundNormal = GetGroundNormal();
            if (groundNormal.HasValue)
            {
                float dotProduct = Vector3.Dot(groundNormal.Value, Vector3.up);
                return Mathf.Abs(dotProduct) > 0.01;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return DistanceToGroundWorld(0.01f) != -1f;
        }
    }

    public Vector3? GetGroundNormal()
    {
        float maxDistance = 1.7f;
        Vector3 castOrigin = transform.position;
        RaycastHit hit;
        if (Physics.Raycast(castOrigin, -transform.up, out hit, maxDistance, terrainLayerMask))
        {
            return hit.normal;
        }
        else
        {
            return null;
        }
    }

    private float DistanceToGround(float maxDistance)
    {
        Vector3 direction = -transform.up;
        Vector3 castOrigin = transform.position - transform.up;

        castOrigin += transform.up * skinWidth;
        maxDistance += skinWidth;

        Ray ray = new Ray(castOrigin, direction);
        Debug.DrawRay(ray.origin, (ray.direction * (maxDistance)), Color.green);

        RaycastHit raycastHit;
        if (Physics.Raycast(castOrigin, -transform.up, out raycastHit, maxDistance, terrainLayerMask))
        {
            return raycastHit.distance - skinWidth;
        }
        else
        {
            return -1;
        }
    }

    private float DistanceToGroundWorld(float maxDistance)
    {
        Vector3 direction = Vector3.down;
        Vector3 castOrigin = (transform.position - (transform.up * 0.5f)) + Vector3.down * 0.5f;

        Debug.DrawRay(castOrigin, Vector3.down, Color.blue, maxDistance);
        RaycastHit hit;
        if (Physics.Raycast(castOrigin, Vector3.down, out hit, maxDistance, terrainLayerMask))
        {
            return hit.distance;
        }
        else
        {
            return -1;
        }
    }

    private Vector3? GetGroundNormalWorld()
    {
        Vector3 direction = Vector3.down;
        Vector3 castOrigin = (transform.position - transform.up) + Vector3.up * 0.5f;

        float maxDistance = 5;
        Debug.DrawRay(castOrigin, Vector3.down, Color.cyan, maxDistance);

        RaycastHit hit;
        if (Physics.Raycast(castOrigin, Vector3.down, out hit, maxDistance, terrainLayerMask))
        {
            Debug.Log("found hit normal");
            return hit.normal;
        }
        else
        {
            Debug.Log("did not find normal");
            return null;
        }
    }

    private void AlignHeadingWithGroundNormal(Vector3 groundNormal)
    {
        Vector3 forward = player.transform.forward.normalized;
        Vector3 newUp = groundNormal.normalized;
        Vector3 left = Vector3.Cross(forward, newUp);
        Vector3 newForward = Vector3.Cross(newUp, left);
        Quaternion newRotation = Quaternion.LookRotation(newForward, newUp);
        player.transform.rotation = newRotation;
    }

    private bool LandingShouldWipeout(Vector3 movement)
    {
        Vector3? groundNormal = player.playerController.GetGroundNormal();
        float similarityTest = Vector3.Dot(player.transform.up, Vector3.up);
        if (!groundNormal.HasValue && similarityTest < 0.95)
        {
            return true;
        }

        Vector3 velocityRelativeToGround = Vector3.ProjectOnPlane(movement, groundNormal.Value);
        if (velocityRelativeToGround.magnitude < 3f)
        {
            return false;
        }

        // this is hack -- if player is straight up and the ground normal is horizontal-ish, don't wipe out
        bool isUpright = player.transform.up.y > 0.8f;
        bool isGroundNormalHorizontal = groundNormal.Value.y < 0.6;
        if (isUpright && isGroundNormalHorizontal)
        {
            return false;
        }

        Quaternion velocityOrientation = Quaternion.LookRotation(velocityRelativeToGround, groundNormal.Value);
        Quaternion playerRotation = player.transform.rotation;

        float similarity = Quaternion.Dot(velocityOrientation, playerRotation);
        float oppositeSimilarity = Quaternion.Dot(velocityOrientation, playerRotation * Quaternion.Euler(0, 180, 0));
        return Mathf.Max(Mathf.Abs(similarity), Mathf.Abs(oppositeSimilarity)) < 0.9f;
    }
}
