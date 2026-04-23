using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LevelAnalytics
{
    // Analytics for each level
    public string levelId;
    public float sessionStartTime;
    public float sessionEndTime;

    // 
    public TimeAnalytics time;
    public JumpAnalytics jumps;
    public CombatAnalytics combat;
    public LifeAnalytics lives;
    public BlockAnalytics blocks;
    public AbilityAnalytics abilities;
    public MovementAnalytics movement;
}
