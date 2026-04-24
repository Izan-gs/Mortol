using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MovementAnalytics : MonoBehaviour
{
    public int directionChanges;
    public int shipMovementsByObstacles;

    public void directionChanged()
    {
        directionChanges++;
    }
    public void shipMoved()
    {
        shipMovementsByObstacles++;
    }
}