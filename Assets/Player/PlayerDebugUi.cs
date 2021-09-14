using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebugUi : MonoBehaviour
{
    [HideInInspector]
    public Player player;

    void OnGUI()
    {
        string content = player.GetStateMachine().currentState != null ? player.GetStateMachine().currentState.GetName() : "(no current state)";
        GUILayout.Label($"<color='black'><size=40>{content}</size></color>");
    }
}
