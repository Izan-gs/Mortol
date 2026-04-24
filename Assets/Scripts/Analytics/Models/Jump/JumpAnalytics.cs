using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JumpAnalytics
{
    public int count;

    public Range height = new Range();
    public Range distance = new Range();

    // Registers height and distance
    public void RegisterJump(float h, float d)
    {
        count++;
        height.Register(h);
        distance.Register(d);
    }
}