using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PUBLIC API
public static class EventBus
{
    // Enemy
    public static void EnemyKilled(EnemyKilledEvent e)
    {
        GameEvents.Emit(e);
    }

    // Ability
    public static void AbilityUsed(AbilityUsedEvent e)
    {
        GameEvents.Emit(e);
    }

    // Life Lost
    public static void LifeLost(LifeLostEvent e)
    {
        GameEvents.Emit(e);
    }

    // Life Gained
    public static void LifeGained(LifeGainEvent e)
    {
        GameEvents.Emit(e);
    }

    // Jump
    public static void Jump(JumpEvent e)
    {
        GameEvents.Emit(e);
    }

    // Movement
    public static void ShipMoved(ShipMovedEvent e)
    {
        GameEvents.Emit(e);
    }
    public static void PlayerChangeDirection(PlayerChangeDirectionEvent e)
    {
        GameEvents.Emit(e);
    }

    // Player State
    public static void PlayerStateChanged(PlayerStateChangedEvent e)
    {
        GameEvents.Emit(e);
    }
}