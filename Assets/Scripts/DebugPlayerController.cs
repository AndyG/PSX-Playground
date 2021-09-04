using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 10f;
    [SerializeField]
    private float turnSpeed = 1f;
    [SerializeField]
    private float minTurnAngle = -90.0f;
    [SerializeField]
    private float maxTurnAngle = 90.0f;

    private float rotationX = 0f;

    void Update()
    {
        KeyboardMove();
        MouseAim();
    }

    private void KeyboardMove() {
        Vector3 dir = new Vector3(0, 0, 0);
        dir.x = Input.GetAxis("Horizontal");
        dir.z = Input.GetAxis("Vertical");
        transform.Translate(dir * moveSpeed * Time.deltaTime);
    }

    private void MouseAim() {
        // get the mouse inputs
        float y = Input.GetAxis("Mouse X") * turnSpeed;
        rotationX += Input.GetAxis("Mouse Y") * turnSpeed;
     
        // clamp the vertical rotation
        rotationX = Mathf.Clamp(rotationX, minTurnAngle, maxTurnAngle);
     
        // rotate the camera
        transform.eulerAngles = new Vector3(-rotationX, transform.eulerAngles.y + y, 0);
    }
}
