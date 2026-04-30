using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSessionAnalyticsFactory
{
    public static GameSessionAnalytics Create(string sessionId)
    {
        return new GameSessionAnalytics
        {
            // SESSION INFO
            sessionId = sessionId,
            sessionStartTime = Time.time,

            // LEVELS PLAYED
            levels = new List<LevelAnalytics>(),

            // GLOBAL AGGREGATED METRICS
            // Nothing yet

            totalPlayTime = 0f
        };
    }
}
