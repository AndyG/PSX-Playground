using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateMachine : StateMachine
{
    public PlayerState_Grounded groundedState;
    public PlayerState_Airborne airborneState;

    [HideInInspector]
    public Player player;

    public PlayerStateMachine(Player player)
    {
        this.player = player;
        groundedState = new PlayerState_Grounded(this);
        airborneState = new PlayerState_Airborne(this);
    }

    protected override State GetInitialState() {
        return groundedState;
    }
}
