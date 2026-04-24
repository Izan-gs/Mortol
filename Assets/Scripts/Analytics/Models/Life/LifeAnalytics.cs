using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

[Serializable]
public class LifeAnalytics
{
    private Dictionary<String, int> gained = new();
    private Dictionary<String, int> lost = new();

    public int remaining;

    public void AddLoss(LifeLossSource loss)// type
    {
        string key = loss.ToString(); // We need the string because JSON needs a string not an enum
        if (!lost.ContainsKey(key))
            lost[key] = 0;

        lost[key]++;
        // Update remaining lives
        remaining--;
    }
    public void AddGain(LifeGainSource gain)
    {
        string key = gain.ToString(); // We need the string because JSON needs a string not an enum
        if(!gained.ContainsKey(key))
            gained[key] = 0;

        gained[key]++;
        // Update remaining lives
        remaining++;
    }

}
