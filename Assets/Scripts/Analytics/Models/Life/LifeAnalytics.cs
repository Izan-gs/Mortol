using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LifeAnalytics
{
    public LifeSourceCounter gained;
    public LifeSourceCounter lost;

    public int gainedByStreak;
    public int remaining;
}
