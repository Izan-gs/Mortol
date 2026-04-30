using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class StatsMapper
{
    public static LevelStatsViewModel Map(LevelAnalytics data)
    {
        return new LevelStatsViewModel
        {
            totalJumps = data.jumps.count,

            maxJumpHeight = data.jumps.height.max,
            minJumpHeight = data.jumps.height.min,

            maxJumpDistance = data.jumps.distance.max,
            minJumpDistance = data.jumps.distance.min,

            enemiesKilled = data.combat.GetKills(),
            abilitiesUsed = data.abilities.GetAbilities(),

            totalTime = data.time.total
        };
    }
}
