using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDirectionalCollisions : MonoBehaviour
{
    [SerializeField]
    private int numRays;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private float bottomCutoff = 0.25f;

    [SerializeField]
    private float topCutoff = 0.25f;

    public float GetDistanceToCollider(Vector3 check)
    {
        float minDistance = int.MaxValue;
        RaycastHit hit;
        foreach (Ray ray in GetRays(check))
        {
            Physics.Raycast(ray.origin, ray.direction, out hit, check.magnitude, layerMask);
            if (hit.distance < minDistance)
            {
                minDistance = hit.distance;
            }
        }

        if (minDistance != int.MaxValue)
        {
            return minDistance - 0.5f;
        }
        else
        {
            return -1f;
        }
    }

    private Ray[] GetRays(Vector3 check)
    {
        int playerHeight = 2;
        Vector3 playerBottomPos = transform.position - (transform.up * playerHeight / 2);
        Vector3 playerTopPos = transform.position + (transform.up * playerHeight / 2);

        Vector3 firstRayPos = playerBottomPos + transform.up * bottomCutoff;
        Vector3 lastRayPos = playerTopPos - transform.up * topCutoff;
        float distance = (lastRayPos - firstRayPos).magnitude;
        float step = distance / numRays;

        Ray[] rays = new Ray[numRays];
        for (int i = 0; i < numRays; i++)
        {
            rays[i] = new Ray(firstRayPos + transform.up * step * i, check.normalized * (check.magnitude + 0.5f));
        }

        return rays;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Ray[] rays = GetRays(GetComponentInParent<Player>().velocity);
        foreach (Ray ray in rays)
        {
            Gizmos.DrawRay(ray);
        }
    }
}
