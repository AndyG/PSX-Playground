using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundNormalTester : MonoBehaviour
{
    private GroundNormalDetector groundNormalDetector;
    private Vector3 lastNormal = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        this.groundNormalDetector = GetComponent<GroundNormalDetector>();
    }

    // Update is called once per frame
    void Update()
    {
        this.lastNormal = this.groundNormalDetector.GetGroundNormal().GetValueOrDefault(Vector3.zero);
    }

    private void OnGUI()
    {
        string lastNormalString = lastNormal != Vector3.zero ? lastNormal.ToString() : "NONE";
        string content = "\nGround Normal: " + lastNormalString;
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }
}
