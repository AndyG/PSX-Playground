using Cinemachine;
using UnityEngine;

public class FreeLookUserInput : MonoBehaviour {

    [SerializeField]
    private float lookSpeedX = 1f;
    [SerializeField]
    private float lookSpeedY = 1f;

    [SerializeField]
    private bool isEnabled = false;

    private CinemachineFreeLook freeLookCam;

    // Use this for initialization
    void Start () {
        freeLookCam = GetComponent<CinemachineFreeLook>();
    }

    public void OnUserMovedCamera(Vector2 direction) {
        if (isEnabled) {
            freeLookCam.m_XAxis.Value += direction.x * lookSpeedX * 180 * Time.deltaTime;
            freeLookCam.m_YAxis.Value += direction.y * lookSpeedY * Time.deltaTime;
        }
    }
}