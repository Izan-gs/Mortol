using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using TMPro;

// Connects the UI Stats Panel with the Analytics Logic
public class StatsPanelUI : MonoBehaviour
{
    public TMP_Text jumpsText;
    public TMP_Text timeText;
    public TMP_Text enemiesText;
    public TMP_Text abilitiesText;

    
    public void Show(LevelAnalytics data)
    {
        if (data == null) return;

        var vm = StatsMapper.Map(data);

        jumpsText.text = $"Jumps: {vm.totalJumps}\nMax: {vm.maxJumpHeight:F2}\nMin: {vm.minJumpHeight:F2}";
        Debug.Log($"TEST: {vm.maxJumpHeight:F2}");
        //timeText.text = $"Time: {vm.totalTime:0.0}s";

        enemiesText.text =
            $"Pig: {GetValueOrZero(vm.enemiesKilled, EnemyType.Pig.ToString())}\n" +
            $"Bee: {GetValueOrZero(vm.enemiesKilled, EnemyType.Bumblebee.ToString())}\n" +
            $"Crocodile: {GetValueOrZero(vm.enemiesKilled, EnemyType.Crocodile.ToString())}";

        abilitiesText.text =
            $"Stone: {GetValueOrZero(vm.abilitiesUsed, AbilityType.Stone.ToString())}\n" +
            $"Explosion: {GetValueOrZero(vm.abilitiesUsed, AbilityType.Explosion.ToString())}\n" +
            $"Arrow: {GetValueOrZero(vm.abilitiesUsed, AbilityType.Arrow.ToString())}";
    }

    // This allows us to show 0 if there are not used entries.
    private string GetValueOrZero(Dictionary<string, int> dict, string key)
    {
        return dict.TryGetValue(key, out int value)
            ? value.ToString()
            : "0";
    }
}