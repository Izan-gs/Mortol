using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAnalyticsFactory
{
    public static LevelAnalytics Create(string levelId)
    {
        return new LevelAnalytics
        {
            levelId = levelId,
            sessionStartTime = Time.time,

            abilities = new AbilityAnalytics(),
            blocks = new BlockAnalytics(),
            combat = new CombatAnalytics(),
            jumps = new JumpAnalytics { height = new Range(), distance = new Range() },
            lives = new LifeAnalytics(),
            movement = new MovementAnalytics(),
            time = new TimeAnalytics()
        };
    }
}