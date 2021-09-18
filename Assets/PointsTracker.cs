using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsTracker : MonoBehaviour
{
    public Player player;
    public UnityEngine.UI.Text trickTextUI;
    public UnityEngine.UI.Text comboScoreUI;
    public UnityEngine.UI.Text totalScoreUI;

    public int pointsPerGrindSecond;
    public int pointsPerAerial180;
    public float comboTimeWindow;

    private PlayerStateMachine playerStateMachine;
    private string lastState;
    private float stateStartTime;
    private Quaternion lastRotation;
    private float totalRotation;
    private float airRotation;
    private int comboScore;
    private int totalScore;

    private void getPlayerStateMachine() {
        if (player != null && playerStateMachine == null) {
            playerStateMachine = player.stateMachine;
            lastState = PlayerState_Wipeout.NAME;
        }
    }

    public void HandleStateChange(State current) {
        string currentState = current.GetName();
        if (currentState != PlayerState_Wipeout.NAME) {
                switch (lastState) {
                    case PlayerState_Grinding.NAME:
                        comboScore += pointsPerGrindSecond * (int)(Time.time - stateStartTime);
                        break;
                    case PlayerState_Airborne.NAME:
                        comboScore += pointsPerAerial180 * (int)Mathf.Abs(totalRotation / 180);
                        totalRotation = 0;
                        break;
                }
        } else {
            totalRotation = 0;
            airRotation = 0;
            comboScore = 0;
            comboScoreUI.text = "0";
        }
        stateStartTime = Time.time;
        trickTextUI.text = "";
        if (currentState == PlayerState_Grinding.NAME) {
            trickTextUI.text = "now grinding";
        }
        if (currentState == PlayerState_Airborne.NAME) {
            trickTextUI.text = "now airborne";
            lastRotation = player.transform.rotation;
        }
    }

    public void TrackPlayer(Player player) {
        this.playerStateMachine = player.stateMachine;
        this.player = player;
    }

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        getPlayerStateMachine();
        if (playerStateMachine == null) {
            return;
        }
        State current = playerStateMachine.currentState;
        string currentState = current.GetName();
        if (currentState != lastState) {
            HandleStateChange(current);
            lastState = currentState;
        }
        switch (currentState) {
            case PlayerState_Grounded.NAME:
                if (Time.time - stateStartTime > comboTimeWindow) {
                    totalScore += comboScore;
                    comboScore = 0;
                    totalScoreUI.text = totalScore.ToString();
                    comboScoreUI.text = "0";
                }
                break;
            case PlayerState_Airborne.NAME:
                Quaternion delta = lastRotation * Quaternion.Inverse(player.transform.rotation);
                totalRotation += delta.eulerAngles.y;
                lastRotation = player.transform.rotation;
                trickTextUI.text = "rotation: " + Mathf.Abs(totalRotation).ToString();
                comboScoreUI.text = (comboScore + (pointsPerAerial180 * (int)Mathf.Abs((totalRotation) / 180))).ToString();
                break;
            case PlayerState_Grinding.NAME:
                trickTextUI.text = "grindtime: " + (Time.time - stateStartTime).ToString();
                comboScoreUI.text = (comboScore + ((int)(pointsPerGrindSecond * (Time.time - stateStartTime)))).ToString();
                break;
        }
    }
}
