using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindableDetector : MonoBehaviour
{
    [SerializeField]
    private float radius;

    [SerializeField]
    private LayerMask grindableLayerMask;

    void OnDrawGizmosSelected()
    {
        // Display the explosion radius when selected
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public Grindable GetGrindable()
    {
        Collider[] grindableColliders = Physics.OverlapSphere(this.transform.position, this.radius, this.grindableLayerMask);
        if (grindableColliders.Length <= 0) return null;

        Collider firstCollider = grindableColliders[0];
        return firstCollider.GetComponent<Grindable>();
    }
}
