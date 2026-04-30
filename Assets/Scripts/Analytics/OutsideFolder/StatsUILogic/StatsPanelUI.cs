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
        var vm = StatsMapper.Map(data);

        jumpsText.text = $"Jumps: {vm.totalJumps}\nMax: {vm.maxJumpHeight:0.0}\nMin: {vm.minJumpHeight:0.0}";

        timeText.text = $"Time: {vm.totalTime:0.0}s";

        enemiesText.text =
            $"Pig: {vm.enemiesKilled["Pig"]}\n" +
            $"Bee: {vm.enemiesKilled["Bumblebee"]}\n" +
            $"Crocodile: {vm.enemiesKilled["Crocodile"]}";

        abilitiesText.text =
            $"Stone: {vm.abilitiesUsed["Stone"]}\n" +
            $"Explosion: {vm.abilitiesUsed["Explosion"]}\n" +
            $"Arrow: {vm.abilitiesUsed["Arrow"]}";
    }
}