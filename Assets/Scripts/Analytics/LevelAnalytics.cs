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
    public AbilityAnalytics abilities;
    public BlockAnalytics blocks;
    public CombatAnalytics combat;
    public JumpAnalytics jumps;
    public LifeAnalytics lives;
    public MovementAnalytics movement;
    public TimeAnalytics time;
}
