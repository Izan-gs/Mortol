using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class StatsMapper
{
    public static LevelStatsViewModel Map(LevelAnalytics data)
    {
        float maxJumpHeight = data.jumps.height.max;
        float minJumpHeight = data.jumps.height.min;
        float maxJumpDistance = data.jumps.distance.max;
        float minJumpDistance = data.jumps.distance.min;
        if (data.jumps.count == 0)
        {
            maxJumpHeight = 0.0f;
            minJumpHeight = 0.0f;
            maxJumpDistance = 0.0f;
            minJumpDistance = 0.0f;
        }

        return new LevelStatsViewModel
        {
            totalJumps = data.jumps.count,

            maxJumpHeight = maxJumpHeight,
            minJumpHeight = minJumpHeight,

            maxJumpDistance = maxJumpDistance,
            minJumpDistance = minJumpDistance,

            enemiesKilled = data.combat.GetKills(),
            abilitiesUsed = data.abilities.GetAbilities(),

            totalTime = data.time.total
        };
    }
}
