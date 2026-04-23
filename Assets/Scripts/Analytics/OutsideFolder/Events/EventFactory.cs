using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EventFactory
{
    public static EnemyKilledEvent EnemyKilled(EnemyType enemy)
    {
        return new EnemyKilledEvent
        {
            enemy = enemy
        };
    }

    public static AbilityUsedEvent AbilityUsed(AbilityType ability)
    {
        return new AbilityUsedEvent
        {
            ability = ability
        };
    }

    public static LifeLostEvent LifeLost(LifeSourceType source)
    {
        return new LifeLostEvent
        {
            source = source
        };
    }

    public static JumpEvent Jump(float height, float distance)
    {
        return new JumpEvent
        {
            height = height,
            distance = distance
        };
    }
    public static PlayerStateChangedEvent PlayerStateChanged(PlayerState state)
    {
        return new PlayerStateChangedEvent
        {
            state = state
        };
    }
}