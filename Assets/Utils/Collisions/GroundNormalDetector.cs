using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundNormalDetector : MonoBehaviour
{

    [SerializeField]
    private float rayDistance = 0.2f;

    [SerializeField]
    private LayerMask layerMask;

    private Vector3? lastNormal = null;

    // Update is called once per frame
    public Vector3? GetGroundNormal()
    {
        RaycastHit raycastHit;
        Vector3 direction = -transform.up;
        Ray ray = new Ray(transform.position, direction);
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.green);

        if (Physics.Raycast(ray, out raycastHit, rayDistance, layerMask))
        {
            Vector3 surfaceNormal = raycastHit.normal;
            this.lastNormal = surfaceNormal;
            return surfaceNormal;
        }
        else
        {
            this.lastNormal = null;
            return null;
        }
    }

    private void OnGUI()
    {
        string lastNormalString = lastNormal.HasValue ? lastNormal.Value.ToString() : "null";
        string content = "\nGround Normal: " + lastNormalString;
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }
}
