using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grindable : MonoBehaviour
{
    [SerializeField]
    public Transform pointA;

    [SerializeField]
    public Transform pointB;

    public GrindableGroup GetGroup()
    {
        return transform.GetComponentInParent<GrindableGroup>();
    }
}
