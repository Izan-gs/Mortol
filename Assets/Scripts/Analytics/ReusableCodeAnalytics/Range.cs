using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Range
{
    public float min = float.MaxValue;
    public float max = float.MinValue;

    public void Register(float value)
    {
        if (value < min) min = value;
        if (value > max) max = value;
    }
}
