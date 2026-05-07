using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

[Serializable]
public class LifeAnalytics
{
    [SerializeField] private List<LifeEntry> gained = new();
    [SerializeField] private List<LifeEntry> lost = new();

    public int remaining;

    public void AddLoss(LifeLossSource loss)// type
    {
        string key = loss.ToString(); // We need the string because JSON needs a string not an enum

        LifeEntry entry = lost.Find(x => x.source == key);

        if (entry == null)
        {
            entry = new LifeEntry
            {
                source = key,
                count = 0
            };

            lost.Add(entry);
        }

        entry.count++;
        // Update remaining lives
        remaining--;
    }
    public void AddGain(LifeGainSource gain)
    {
        string key = gain.ToString(); // We need the string because JSON needs a string not an enum

        LifeEntry entry = gained.Find(x => x.source == key);

        if (entry == null)
        {
            entry = new LifeEntry
            {
                source = key,
                count = 0
            };

            gained.Add(entry);
        }

        entry.count++;
        // Update remaining lives
        remaining++;
    }
}
