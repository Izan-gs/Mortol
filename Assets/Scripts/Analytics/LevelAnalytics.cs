using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All the level metrics will be saved in the LevelAnalytics instance
[Serializable]
public class LevelAnalytics
{
    // Analytics For Each Level
    public string levelId;
    public float sessionStartTime;
    public float sessionEndTime;

    // Game Metrics Per Level
    public AbilityAnalytics abilities;
    public BlockAnalytics blocks;
    public CombatAnalytics combat;
    public JumpAnalytics jumps;
    public LifeAnalytics lives;
    public MovementAnalytics movement;
    public TimeAnalytics time;
}
