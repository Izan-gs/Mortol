using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TimeAnalytics
{
    public float total;

    public float air;
    public float ground;
    public float idle;
    public float moving;
    public float fallingFromShip;

    public PlayerState currentState;

    public void SetState(PlayerState state)
    {
        currentState = state;
    }

    public void Tick(float dt)
    {
        total += dt;

        if (currentState == PlayerState.Air) air += dt;
        else if (currentState == PlayerState.Ground) ground += dt;
        else if (currentState == PlayerState.Idle) idle += dt;
        else if (currentState == PlayerState.Moving) moving += dt;
        else if (currentState == PlayerState.FallingFromShip) fallingFromShip += dt;
    }
}