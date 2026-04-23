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

            time = new TimeAnalytics(),
            jumps = new JumpAnalytics { height = new Range(), distance = new Range() },
            combat = new CombatAnalytics { enemiesKilled = new EnemyCounter() },
            lives = new LifeAnalytics { gained = new LifeSourceCounter(), lost = new LifeSourceCounter() },
            blocks = new BlockAnalytics(),
            abilities = new AbilityAnalytics(),
            movement = new MovementAnalytics()
        };
    }
}