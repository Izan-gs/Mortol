using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// INTERNAL DISPATCHER
public static class GameEvents
{
    // Enemies
    public static event Action<EnemyKilledEvent> OnEnemyKilled;

    public static void Emit(EnemyKilledEvent e)
    {
        OnEnemyKilled?.Invoke(e);
    }

    // Abilities
    public static event Action<AbilityUsedEvent> OnAbilityUsed;

    public static void Emit(AbilityUsedEvent e)
    {
        OnAbilityUsed?.Invoke(e);
    }

    // Life
    public static event Action<LifeLostEvent> OnLifeLost;

    public static void Emit(LifeLostEvent e)
    {
        OnLifeLost?.Invoke(e);
    }

    public static event Action<LifeGainEvent> OnLifeGain;

    public static void Emit(LifeGainEvent e)
    {
        OnLifeGain?.Invoke(e);
    }

    // Jump
    public static event Action<JumpEvent> OnJump;

    public static void Emit(JumpEvent e)
    {
        OnJump?.Invoke(e);
    }

    // Movement
    public static event Action<ShipMovedEvent> OnShipMoved;

    public static void Emit(ShipMovedEvent e)
    {
        OnShipMoved?.Invoke(e);
    }

    public static event Action<PlayerChangeDirectionEvent> OnPlayerChangedDirection;

    public static void Emit(PlayerChangeDirectionEvent e)
    {
        OnPlayerChangedDirection?.Invoke(e);
    }

    // Player States
    public static event Action<PlayerStateChangedEvent> OnPlayerStateChanged;

    public static void Emit(PlayerStateChangedEvent e)
    {
        OnPlayerStateChanged?.Invoke(e);
    }
}
