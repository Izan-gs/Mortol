using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

[Serializable]
public class CombatAnalytics
{
    private Dictionary<String, int> kills = new();

    public void RegisterKill(EnemyType enemy)
    {
        string key = enemy.ToString(); // We need the string because JSON needs a string not an enum

        if (!kills.ContainsKey(key))
            kills[key] = 0;

        kills[key]++;
    }
}
