using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All the game session metrics will be saved in the GameSessionAnalytics instance
[Serializable]
public class GameSessionAnalytics
{
    // SESSION INFO
    public string sessionId;
    public float sessionStartTime;
    public float sessionEndTime;

    // LEVELS PLAYED
    public List<LevelAnalytics> levels;

    // GLOBAL AGGREGATED METRICS
    // Nothing yet

    public float totalPlayTime;

    // Adds the level to the list and updates the metrics
    public void AddLevel(LevelAnalytics level)
    {
        levels.Add(level);
    }
}
