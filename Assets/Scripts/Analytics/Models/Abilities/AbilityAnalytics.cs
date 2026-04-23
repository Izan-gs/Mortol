using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AbilityAnalytics
{
    public int stone;
    public int explosion;
    public int arrow;

    public void RegisterUse(AbilityType ability)
    {
        if (ability == AbilityType.Stone) stone++;
        else if (ability == AbilityType.Explosion) explosion++;
        else if (ability == AbilityType.Arrow) arrow++;
    }
}
