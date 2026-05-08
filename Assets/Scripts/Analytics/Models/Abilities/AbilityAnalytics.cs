using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AbilityAnalytics
{
    [SerializeField]
    private List<AbilityEntry> usage = new();

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

            usage.Add(entry);
        }

        entry.count++;
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
