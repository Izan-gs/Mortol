using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class JumpAnalytics
{
    public int count;

    public Range height;
    public Range distance;

    public void RegisterJump(float h, float d)
    {
        count++;
        height.Register(h);
        distance.Register(d);
    }
}