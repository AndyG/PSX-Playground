using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrindableGroup : MonoBehaviour
{
    [SerializeField]
    private List<Grindable> grindables;

    [SerializeField]
    private float grindablePointSearchDistance = 0.5f;

    /**
    * Return the next grindable that is not the current grindable based on the position of the player.
    */
    public Grindable GetNextGrindable(Grindable curGrindable, Transform playerFoot)
    {
        foreach (Grindable grindable in this.grindables)
        {
            // exclude the current grindable
            if (System.Object.ReferenceEquals(grindable, curGrindable))
            {
                continue;
            }

            if (Vector3.Distance(grindable.pointA.transform.position, playerFoot.transform.position) < grindablePointSearchDistance)
            {
                return grindable;
            }

            if (Vector3.Distance(grindable.pointB.transform.position, playerFoot.transform.position) < grindablePointSearchDistance)
            {
                return grindable;
            }
        }

        return null;
    }
}
