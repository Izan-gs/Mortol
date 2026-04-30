using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelStatsViewModel
{
    public int totalJumps;
    public float maxJumpHeight;
    public float minJumpHeight;

    public float maxJumpDistance;
    public float minJumpDistance;

    public Dictionary<string, int> enemiesKilled;
    public Dictionary<string, int> abilitiesUsed;

    public float totalTime;
}
