using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AbilityAnalytics
{
    public List<AbilityEntry> usage = new();

    public AbilityEntry[] usageArray;



    public void RegisterUse(AbilityType ability)
    {
        string key = ability.ToString();

        AbilityEntry entry = usage.Find(x => x.ability == key);

        if (entry == null)
        {
            entry = new AbilityEntry
            {
                ability = key,
                count = 0
            };
            Debug.Log("RegisterWasNull");

            usage.Add(entry);
        }

        entry.count++;
        usageArray = usage.ToArray();
    }

    // Getter
    public Dictionary<string, int> GetAbilities()
    {
        Dictionary<string, int> dict = new();

        foreach (var item in usage)
        {
            dict[item.ability] = item.count;
        }

        return dict;
    }
}
