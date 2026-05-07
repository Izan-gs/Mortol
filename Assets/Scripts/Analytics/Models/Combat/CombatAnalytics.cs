using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

[Serializable]
public class CombatAnalytics
{
    [SerializeField]
    private List<KillEntry> kills = new();

    public void RegisterKill(EnemyType enemy)
    {
        string key = enemy.ToString();

        KillEntry entry = kills.Find(x => x.enemy == key);

        if (entry == null)
        {
            entry = new KillEntry
            {
                enemy = key,
                count = 0
            };

            kills.Add(entry);
        }

        entry.count++;
    }
    // Getter
    public Dictionary<string, int> GetKills()
    {
        Dictionary<string, int> dict = new();

        foreach (var item in kills)
        {
            dict[item.enemy] = item.count;
        }

        return dict;
    }
}
