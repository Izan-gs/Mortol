using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LifeSourceCounter
{
    // lives gained from OR lives lost by
    public int pig;
    public int bumblebee;
    public int crocodile;

    // lives lost by
    public int stone;
    public int explosion;
    public int arrow;
    public int spikes;
    public int spikedBalls;

    // GAINERS
    public void AddGain(LifeSourceType source)
    {
        if (source == LifeSourceType.Pig) pig++;
        else if (source == LifeSourceType.Bumblebee) bumblebee++;
        else if (source == LifeSourceType.Crocodile) crocodile++;
    }

    // LOSERS
    public void AddLoss(LifeSourceType source)
    {
        if (source == LifeSourceType.Spikes) spikes++;
        else if (source == LifeSourceType.Pig) pig++;
        else if (source == LifeSourceType.Bumblebee) bumblebee++;
        else if (source == LifeSourceType.Crocodile) crocodile++;
    }
}
