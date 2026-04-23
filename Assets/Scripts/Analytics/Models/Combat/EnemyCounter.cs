using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyCounter
{
    private int pig;
    private int bumblebee;
    private int crocodile;

    public void RegisterKill(EnemyType enemy)
    {
        if (enemy == EnemyType.Pig) pig++;
        else if (enemy == EnemyType.Bumblebee) bumblebee++;
        else if (enemy == EnemyType.Crocodile) crocodile++;
    }

    public int GetKills(EnemyType enemy)
    {
        if (enemy == EnemyType.Pig) return pig;
        if (enemy == EnemyType.Bumblebee) return bumblebee;
        if (enemy == EnemyType.Crocodile) return crocodile;
        return 0;
    }
}