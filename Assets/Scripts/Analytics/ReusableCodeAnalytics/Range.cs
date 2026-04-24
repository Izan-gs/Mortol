using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Range
{
    public float min;
    public float max;
    public float sum;
    public int count;
    public float average;

    public Range()
    {
        min = float.MaxValue;
        max = float.MinValue;
    }

    public void Register(float value)
    {
        if (value < min) min = value;
        if (value > max) max = value;

        sum += value;
        count++;

        average = sum / count;
    }
}
