using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using TMPro;

public class StatsPanelUI : MonoBehaviour
{
    public TMP_Text jumpsText;
    public TMP_Text timeText;
    public TMP_Text enemiesText;
    public TMP_Text abilitiesText;

    
    public void Show(LevelAnalytics data)
    {
        if (data == null) return;
        gameObject.SetActive(true); // Opens the panel

        var vm = StatsMapper.Map(data);

        jumpsText.text = $"Jumps: {vm.totalJumps}\nMax: {vm.maxJumpHeight:F2}\nMin: {vm.minJumpHeight:F2}";
        Debug.Log($"TEST: {vm.maxJumpHeight:F2}");
        timeText.text = $"Time: {vm.totalTime:0.0}s";

        enemiesText.text =
            $"Pig: {vm.enemiesKilled[EnemyType.Pig.ToString()]}\n" +
            $"Bee: {vm.enemiesKilled[EnemyType.Bumblebee.ToString()]}\n" +
            $"Crocodile: {vm.enemiesKilled[EnemyType.Crocodile.ToString()]}";

        abilitiesText.text =
            $"Stone: {vm.abilitiesUsed[AbilityType.Stone.ToString()]}\n" +
            $"Explosion: {vm.abilitiesUsed[AbilityType.Explosion.ToString()]}\n" +
            $"Arrow: {vm.abilitiesUsed[AbilityType.Arrow.ToString()]}";
    }
}