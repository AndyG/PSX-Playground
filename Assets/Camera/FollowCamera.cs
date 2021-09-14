using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public bool follow = true;

    [SerializeField]
    public Player player;

    [SerializeField]
    public Transform lookAtTarget;

    [SerializeField]
    public Vector3 offset;

    [SerializeField]
    private float maxSpeed = 5f;

    [SerializeField]
    private float smoothTime;

    [SerializeField]
    private float lookSpeed = 180f;

    private Vector3 velocity = Vector3.zero;

    // Update is called once per frame
    void Update()
    {
        if (follow && player != null)
        {
            Quaternion playerMovementRotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(player.velocity, Vector3.up), Vector3.up);
            Vector3 rotatedOffset = playerMovementRotation * offset;
            Vector3 targetPosition = player.transform.position + rotatedOffset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        }

        if (lookAtTarget != null)
        {
            Quaternion lookAtTargetRotation = Quaternion.LookRotation(lookAtTarget.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookAtTargetRotation, lookSpeed * Time.deltaTime);
        }
    }
}
