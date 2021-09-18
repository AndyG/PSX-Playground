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

    public void Start()
    {
        player = GetComponent<Player>();
    }

    public void Move(Vector3 movement)
    {
        if (!IsGrounded())
        {
            // prioritize local
            float distanceToGround = DistanceToGround(Mathf.Abs(movement.y));
            if (movement.y < 0f && distanceToGround != -1 && distanceToGround < Mathf.Abs(movement.y))
            {
                movement.y = -distanceToGround;
            }
            else if (movement.y < 0f)
            {
                float distanceToGroundWorld = DistanceToGroundWorld(5f);
                if (distanceToGroundWorld != -1 && distanceToGroundWorld < Mathf.Abs(movement.y))
                {
                    bool isMovingIntoGround = distanceToGround < Mathf.Abs(movement.y);
                    movement.y = -distanceToGround - skinWidth;
                    Vector3? groundNormal = GetGroundNormalWorld();
                    if (groundNormal.HasValue)
                    {
                        AlignHeadingWithGroundNormal(groundNormal.Value);
                    }
                }
            }
        }

        transform.position += movement;

        // check if in ground
        RaycastHit hit;
        bool isInGround = Physics.Raycast(transform.position, -transform.up, out hit, 1.1f, terrainLayerMask);
        if (isInGround)
        {
            float distance = hit.distance;
            if (distance < 1)
            {
                float difference = 1f - distance;
                Vector3 translation = difference * transform.up;
                transform.position += translation;
            }
        }

        DrawDistanceToGroundWorldDebugRay(5f);
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

    private void DrawDistanceToGroundWorldDebugRay(float maxDistance)
    {
        Vector3 direction = Vector3.down;
        Vector3 castOrigin = transform.position - transform.up;

        Ray ray = new Ray(castOrigin, direction);
        Debug.DrawRay(ray.origin, (ray.direction * (maxDistance)), Color.cyan);
    }

    private float DistanceToGroundWorld(float maxDistance)
    {
        Vector3 direction = Vector3.down;
        Vector3 castOrigin = transform.position - transform.up;

        RaycastHit raycastHit;
        if (Physics.Raycast(castOrigin, Vector3.down, out raycastHit, maxDistance, terrainLayerMask))
        {
            return raycastHit.distance;
        }
        else
        {
            return -1;
        }
    }

    private Vector3? GetGroundNormalWorld()
    {
        Vector3 castOrigin = transform.position - transform.up + Vector3.up * 0.2f;
        float maxDistance = 0.21f;
        RaycastHit hit;
        if (Physics.Raycast(castOrigin, Vector3.down, out hit, maxDistance, terrainLayerMask))
        {
            return hit.normal;
        }
        else
        {
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
