using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[Serializable]
public class AbilityAnalytics
{
    private Dictionary<String, int> usage = new();

    public void RegisterUse(AbilityType ability)
    {
        string key = ability.ToString(); // We need the string because JSON needs a string not an enum

        if (!usage.ContainsKey(key))
            usage[key] = 0;

        usage[key]++;
    }

    // Getter
    public Dictionary<string, int> GetAbilities()
    {
        return usage;
    }
}
