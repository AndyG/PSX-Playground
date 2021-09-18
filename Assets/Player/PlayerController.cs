using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float skinWidth = 0.02f;

    public float groundCheckRayDistance = 0.5f;
    public LayerMask terrainLayerMask;

    public Transform groundCheckerFront;
    public Transform groundCheckerBack;

    public Transform groundCheckerSphere;

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
        Debug.Log("movement y: " + movement.y);
        if (!IsGrounded())
        {
            // prioritize local
            float distanceToGround = DistanceToGround(Mathf.Abs(movement.y));
            if (movement.y < 0f && distanceToGround != -1 && distanceToGround < Mathf.Abs(movement.y))
            {
                // is this right? seems wrong
                movement.y = -distanceToGround;
            }
            else if (movement.y < 0f)
            {
                float distanceToGroundWorld = DistanceToGroundWorld(5f);
                Debug.Log("distance to ground world: " + distanceToGroundWorld);
                if (distanceToGroundWorld != -1 && distanceToGroundWorld < Mathf.Abs(movement.y))
                {
                    movement.y = -distanceToGroundWorld;
                    shouldWipeOut = true;
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
        return DistanceToGround(0.01f) != -1f || DistanceToGroundWorld(0.01f) != -1f;
    }

    private RaycastHit? CheckGround(Transform groundCheckPosition)
    {
        RaycastHit raycastHit;
        Vector3 direction = -transform.up;
        Ray ray = new Ray(transform.position, direction);
        Debug.DrawRay(ray.origin, ray.direction * groundCheckRayDistance, Color.green);

        if (Physics.Raycast(ray, out raycastHit, groundCheckRayDistance, terrainLayerMask))
        {
            return raycastHit;
        }
        else
        {
            return null;
        }
    }

    public Vector3? GetGroundNormal()
    {
        Vector3 castOrigin = transform.position;
        float maxDistance = 1.7f;
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
}
